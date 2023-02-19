//
//  PcapNostaleClient.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Extensions;
using NosSmooth.Core.Packets;
using NosSmooth.Cryptography;
using NosSmooth.Cryptography.Extensions;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using PacketDotNet;
using Remora.Results;
using SharpPcap;
using SharpPcap.LibPcap;

namespace NosSmooth.Pcap;

/// <summary>
/// A NosTale client that works by capturing packets.
/// </summary>
/// <remarks>
/// Sending packets means the same number of packet will appear twice.
/// That may be detected by the server and the server may suspect
/// something malicious is going on.
/// </remarks>
public class PcapNostaleClient : BaseNostaleClient
{
    private readonly Process _process;
    private readonly Encoding _encoding;
    private readonly PcapNostaleManager _pcapManager;
    private readonly ProcessTcpManager _processTcpManager;
    private readonly IPacketHandler _handler;
    private readonly ILogger<PcapNostaleClient> _logger;
    private readonly PcapNostaleOptions _options;
    private CryptographyManager _crypto;
    private CancellationToken? _stoppingToken;
    private bool _running;
    private LibPcapLiveDevice? _lastDevice;
    private TcpConnection _connection;
    private PhysicalAddress? _localAddress;
    private PhysicalAddress? _remoteAddress;
    private long _lastPacketIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="PcapNostaleClient"/> class.
    /// </summary>
    /// <param name="process">The process to look for.</param>
    /// <param name="initialEncryptionKey">The current encryption key of the world connection, if known. Zero if unknown.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="pcapManager">The pcap manager.</param>
    /// <param name="processTcpManager">The process manager.</param>
    /// <param name="handler">The packet handler.</param>
    /// <param name="commandProcessor">The command processor.</param>
    /// <param name="options">The options.</param>
    /// <param name="logger">The logger.</param>
    public PcapNostaleClient
    (
        Process process,
        int initialEncryptionKey,
        Encoding encoding,
        PcapNostaleManager pcapManager,
        ProcessTcpManager processTcpManager,
        IPacketHandler handler,
        CommandProcessor commandProcessor,
        IOptions<PcapNostaleOptions> options,
        ILogger<PcapNostaleClient> logger
    )
        : base(commandProcessor)
    {
        _process = process;
        _encoding = encoding;
        _pcapManager = pcapManager;
        _processTcpManager = processTcpManager;
        _handler = handler;
        _logger = logger;
        _options = options.Value;
        _crypto = new CryptographyManager();
        _crypto.EncryptionKey = initialEncryptionKey;
    }

    /// <inheritdoc />
    public override async Task<Result> RunAsync(CancellationToken stopRequested = default)
    {
        if (_running)
        {
            return Result.FromSuccess();
        }

        _running = true;

        _stoppingToken = stopRequested;
        TcpConnection? lastConnection = null;
        TcpConnection? reverseLastConnection = null;
        try
        {
            await _processTcpManager.RegisterProcess(_process.Id);
            _pcapManager.AddClient();

            while (!stopRequested.IsCancellationRequested)
            {
                if (_process.HasExited)
                {
                    break;
                }

                var connections = await _processTcpManager.GetConnectionsAsync(_process.Id);
                TcpConnection? connection = connections.Count > 0 ? connections[0] : null;

                if (lastConnection != connection)
                {
                    if (lastConnection is not null)
                    {
                        _pcapManager.UnregisterConnection(lastConnection.Value, this);
                        _crypto.EncryptionKey = 0;
                    }
                    if (reverseLastConnection is not null)
                    {
                        _pcapManager.UnregisterConnection(reverseLastConnection.Value, this);
                    }

                    if (connection is not null)
                    {
                        var conn = connection.Value;
                        var reverseConn = new TcpConnection
                            (conn.RemoteAddr, conn.RemotePort, conn.LocalAddr, conn.LocalPort);

                        _connection = conn;

                        _pcapManager.RegisterConnection(conn, this);
                        _pcapManager.RegisterConnection(reverseConn, this);

                        lastConnection = conn;
                        reverseLastConnection = reverseConn;
                    }
                    else
                    {
                        lastConnection = null;
                        reverseLastConnection = null;
                    }
                }

                await Task.Delay(TimeSpan.FromMilliseconds(_options.ProcessRefreshInterval), stopRequested);
            }
        }
        catch (OperationCanceledException)
        {
            // ignored
        }
        catch (Exception e)
        {
            return e;
        }
        finally
        {
            await _processTcpManager.UnregisterProcess(_process.Id);
            _pcapManager.RemoveClient();
            if (lastConnection is not null)
            {
                _pcapManager.UnregisterConnection(lastConnection.Value, this);
            }

            if (reverseLastConnection is not null)
            {
                _pcapManager.UnregisterConnection(reverseLastConnection.Value, this);
            }

            _running = false;
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public override async Task<Result> SendPacketAsync(string packetString, CancellationToken ct = default)
    {
        if (_lastDevice is null)
        {
            return new NotSupportedError("The device was not captured yet, cannot send packet.");
        }

        if (_lastDevice?.Loopback ?? false)
        {
            return new NotSupportedError("Loopback devices cannot send or receive packets.");
        }

        var ethPacket = new EthernetPacket(_localAddress, _remoteAddress, EthernetType.IPv4);
        var ipPacket = new IPv4Packet(new IPAddress(_connection.LocalAddr), new IPAddress(_connection.RemoteAddr));
        var tcpPacket = new TcpPacket((ushort)_connection.LocalPort, (ushort)_connection.RemotePort);
        tcpPacket.PayloadData = _crypto.ClientWorld.Encrypt(_lastPacketIndex + " " + packetString, _encoding);
        ethPacket.PayloadPacket = ipPacket;
        ipPacket.PayloadPacket = tcpPacket;

        try
        {
            _lastDevice?.SendPacket(ethPacket);
        }
        catch (Exception e)
        {
            return e;
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public override async Task<Result> ReceivePacketAsync(string packetString, CancellationToken ct = default)
    {
        if (_lastDevice is null)
        {
            return new NotSupportedError("The device was not captured yet, cannot receive packet.");
        }

        if (_lastDevice?.Loopback ?? false)
        {
            return new NotSupportedError("Loopback devices cannot send or receive packets.");
        }

        var ethPacket = new EthernetPacket(_remoteAddress, _localAddress, EthernetType.IPv4);
        var ipPacket = new IPv4Packet(new IPAddress(_connection.RemoteAddr), new IPAddress(_connection.LocalAddr));
        var tcpPacket = new TcpPacket((ushort)_connection.RemotePort, (ushort)_connection.LocalPort);
        tcpPacket.PayloadData = _crypto.ServerWorld.Encrypt(packetString, _encoding);
        ipPacket.PayloadPacket = tcpPacket;
        ethPacket.PayloadPacket = ipPacket;

        try
        {
            _lastDevice?.SendPacket(ethPacket);
        }
        catch (Exception e)
        {
            return e;
        }

        return Result.FromSuccess();
    }

    /// <summary>
    /// Called when an associated packet has been obtained.
    /// </summary>
    /// <param name="device">The device the packet was received at.</param>
    /// <param name="connection">The connection that obtained the packet.</param>
    /// <param name="payloadData">The raw payload data of the packet.</param>
    /// <param name="ethernetPacket">The ethernet packet containing source and destination hardware addresses.</param>
    internal void OnPacketArrival
    (
        LibPcapLiveDevice? device,
        TcpConnection connection,
        byte[] payloadData,
        EthernetPacket? ethernetPacket
    )
    {
        _lastDevice = device;

        string data;
        PacketSource source;
        bool mayContainPacketId = false;

        if (connection.LocalAddr == _connection.LocalAddr && connection.LocalPort == _connection.LocalPort)
        { // sent packet
            _localAddress = ethernetPacket?.SourceHardwareAddress;
            _remoteAddress = ethernetPacket?.DestinationHardwareAddress;

            source = PacketSource.Client;
            mayContainPacketId = true;
            data = _crypto.DecryptUnknownServerPacket(payloadData, _encoding);
        }
        else
        { // received packet
            _remoteAddress = ethernetPacket?.SourceHardwareAddress;
            _localAddress = ethernetPacket?.DestinationHardwareAddress;

            source = PacketSource.Server;
            data = _crypto.DecryptUnknownClientPacket(payloadData, _encoding);
        }

        if (data.Length > 0)
        {
            foreach (ReadOnlySpan<char> line in data.SplitLines())
            {
                var linePacket = line;
                if (mayContainPacketId)
                {
                    var spaceIndex = linePacket.IndexOf(' ');
                    if (spaceIndex != -1)
                    {
                        var beginning = linePacket.Slice(0, spaceIndex);

                        if (int.TryParse(beginning, out var packetIndex))
                        {
                            _lastPacketIndex = packetIndex;
                            linePacket = linePacket.Slice(spaceIndex + 1);
                        }
                    }
                }

                var lineString = linePacket.ToString();
                Task.Run(() => ProcessPacketAsync(source, lineString));
            }
        }
    }

    private async Task ProcessPacketAsync(PacketSource type, string packetString)
    {
        try
        {
            var result = await _handler.HandlePacketAsync(this, type, packetString);

            if (!result.IsSuccess)
            {
                _logger.LogError("There was an error whilst handling packet {packetString}", packetString);
                _logger.LogResultError(result);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "The process packet threw an exception");
        }
    }
}