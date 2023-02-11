//
//  PcapNostaleClient.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Packets;
using NosSmooth.Cryptography;
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
    private CryptographyManager _crypto;
    private int _localPort;
    private long _localAddr;
    private CancellationToken? _stoppingToken;
    private bool _running;

    /// <summary>
    /// Initializes a new instance of the <see cref="PcapNostaleClient"/> class.
    /// </summary>
    /// <param name="process">The process to look for.</param>
    /// <param name="encryptionKey">The current encryption key of the world connection, if known. Zero if unknown.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="pcapManager">The pcap manager.</param>
    /// <param name="processTcpManager">The process manager.</param>
    /// <param name="handler">The packet handler.</param>
    /// <param name="commandProcessor">The command processor.</param>
    public PcapNostaleClient
    (
        Process process,
        int encryptionKey,
        Encoding encoding,
        PcapNostaleManager pcapManager,
        ProcessTcpManager processTcpManager,
        IPacketHandler handler,
        CommandProcessor commandProcessor
    )
        : base(commandProcessor)
    {
        _process = process;
        _encoding = encoding;
        _pcapManager = pcapManager;
        _processTcpManager = processTcpManager;
        _handler = handler;
        _crypto = new CryptographyManager();
        _crypto.EncryptionKey = encryptionKey;
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

                var connection = (await _processTcpManager.GetConnectionsAsync(_process.Id)).Cast<TcpConnection?>()
                    .FirstOrDefault();

                if (lastConnection != connection)
                {
                    if (lastConnection is not null)
                    {
                        _pcapManager.UnregisterConnection(lastConnection.Value);
                        _crypto.EncryptionKey = 0;
                    }
                    if (reverseLastConnection is not null)
                    {
                        _pcapManager.UnregisterConnection(reverseLastConnection.Value);
                    }

                    if (connection is not null)
                    {
                        var conn = connection.Value;
                        var reverseConn = new TcpConnection
                            (conn.RemoteAddr, conn.RemotePort, conn.LocalAddr, conn.LocalPort);

                        _localAddr = conn.LocalAddr;
                        _localPort = conn.LocalPort;

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

                await Task.Delay(TimeSpan.FromMilliseconds(10), stopRequested);
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
                _pcapManager.UnregisterConnection(lastConnection.Value);
            }

            if (reverseLastConnection is not null)
            {
                _pcapManager.UnregisterConnection(reverseLastConnection.Value);
            }

            _running = false;
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public override Task<Result> SendPacketAsync(string packetString, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override Task<Result> ReceivePacketAsync(string packetString, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Called when an associated packet has been obtained.
    /// </summary>
    /// <param name="connection">The connection that obtained the packet.</param>
    /// <param name="payloadData">The raw payload data of the packet.</param>
    internal void OnPacketArrival(TcpConnection connection, byte[] payloadData)
    {
        string data;
        PacketSource source;
        bool containsPacketId = false;

        if (connection.LocalAddr == _localAddr && connection.LocalPort == _localPort)
        { // sent packet
            source = PacketSource.Client;
            if (_crypto.EncryptionKey == 0)
            {
                var worldDecrypted = _crypto.ServerWorld.Decrypt(payloadData, _encoding).Trim();

                var splitted = worldDecrypted.Split(' ');
                if (splitted.Length == 2 && int.TryParse(splitted[1], out var encryptionKey))
                { // possibly first packet from world
                    _crypto.EncryptionKey = encryptionKey;
                    data = worldDecrypted;
                    containsPacketId = true;
                }
                else
                { // doesn't look like first packet from world, so assume login.
                    data = _crypto.ServerLogin.Decrypt(payloadData, _encoding);
                }
            }
            else
            {
                data = _crypto.ServerWorld.Decrypt(payloadData, _encoding);
                containsPacketId = true;
            }
        }
        else
        { // received packet
            source = PacketSource.Server;
            if (_crypto.EncryptionKey == 0)
            { // probably login
                data = _crypto.ClientLogin.Decrypt(payloadData, _encoding);
            }
            else
            {
                data = _crypto.ClientWorld.Decrypt(payloadData, _encoding);
            }
        }

        if (data.Length > 0)
        {
            foreach (var line in data.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var linePacket = line;
                if (containsPacketId)
                {
                    linePacket = line.Substring(line.IndexOf(' ') + 1);
                }

                _handler.HandlePacketAsync(this, source, linePacket.Trim(), _stoppingToken ?? default);
            }

        }
    }
}