//
//  PcapNostaleManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

namespace NosSmooth.Pcap;

/// <summary>
/// Captures packets, distributes them to Pcap clients.
/// </summary>
public class PcapNostaleManager
{
    private readonly ILogger<PcapNostaleManager> _logger;
    private readonly PcapNostaleOptions _options;
    private readonly ConcurrentDictionary<TcpConnection, ConnectionData> _connections;
    private readonly ConcurrentDictionary<TcpConnection, List<PcapNostaleClient>> _clients;
    private Task? _deletionTask;
    private CancellationTokenSource? _deletionTaskCancellationSource;
    private int _clientsCount;
    private bool _started;

    /// <summary>
    /// Initializes a new instance of the <see cref="PcapNostaleManager"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The options.</param>
    public PcapNostaleManager(ILogger<PcapNostaleManager> logger, IOptions<PcapNostaleOptions> options)
    {
        _logger = logger;
        _options = options.Value;
        _connections = new ConcurrentDictionary<TcpConnection, ConnectionData>();
        _clients = new ConcurrentDictionary<TcpConnection, List<PcapNostaleClient>>();
    }

    /// <summary>
    /// Add a pcap client.
    /// </summary>
    internal void AddClient()
    {
        var count = Interlocked.Increment(ref _clientsCount);

        if (count == 1)
        {
            StartCapturing();
        }
    }

    /// <summary>
    /// Remove a pcap client.
    /// </summary>
    /// <remarks>
    /// When no clients are left, packet capture will be stopped.
    /// </remarks>
    internal void RemoveClient()
    {
        var count = Interlocked.Decrement(ref _clientsCount);

        if (count == 0)
        {
            Stop();
        }
    }

    /// <summary>
    /// Associate the given connection with the given client.
    /// </summary>
    /// <param name="connection">The connection to associate.</param>
    /// <param name="client">The client to associate the connection with.</param>
    internal void RegisterConnection(TcpConnection connection, PcapNostaleClient client)
    {
        _clients.AddOrUpdate
        (
            connection,
            _ =>
            {
                var clients = new List<PcapNostaleClient>();
                clients.Add(client);
                return clients;
            },
            (_, currentClients) =>
            {
                var clients = new List<PcapNostaleClient>(currentClients);
                clients.Add(client);
                return clients;
            }
        );

        if (_connections.TryGetValue(connection, out var data))
        {
            foreach (var sniffedPacket in data.SniffedData)
            {
                client.OnPacketArrival(null, connection, sniffedPacket);
            }
        }
    }

    /// <summary>
    /// Disassociate the given connection.
    /// </summary>
    /// <param name="connection">The connection to disassociate.</param>
    /// <param name="client">The client to unregister.</param>
    internal void UnregisterConnection(TcpConnection connection, PcapNostaleClient client)
    {
        _clients.AddOrUpdate
        (
            connection,
            (c) => new List<PcapNostaleClient>(),
            (c1, c2) =>
            {
                var clients = new List<PcapNostaleClient>(c2);
                clients.Remove(client);
                return clients;
            }
        );
    }

    private void Stop()
    {
        if (!_started)
        {
            return;
        }

        _started = false;
        foreach (var device in LibPcapLiveDeviceList.Instance)
        {
            device.StopCapture();
        }

        var task = _deletionTask;
        _deletionTask = null;

        _deletionTaskCancellationSource?.Cancel();
        _deletionTaskCancellationSource?.Dispose();
        _deletionTaskCancellationSource = null;

        task?.GetAwaiter().GetResult();
        task?.Dispose();
        _connections.Clear();
        _clients.Clear();
    }

    /// <summary>
    /// Start capturing packets from all devices.
    /// </summary>
    public void StartCapturing()
    {
        if (_started)
        {
            return;
        }

        _started = true;
        _deletionTaskCancellationSource = new CancellationTokenSource();
        _deletionTask = Task.Run(() => DeletionTask(_deletionTaskCancellationSource.Token));

        foreach (var device in LibPcapLiveDeviceList.Instance)
        {
            if (!device.Opened)
            {
                device.Open();
            }

            device.Filter = "ip and tcp";
            device.OnPacketArrival += DeviceOnOnPacketArrival;
            device.StartCapture();
        }
    }

    private void DeviceOnOnPacketArrival(object sender, PacketCapture e)
    {
        try
        {
            DeviceOnPacketArrivalInner(e);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OnPacketArrival has produced an exception");
        }
    }

    private void DeviceOnPacketArrivalInner(PacketCapture e)
    {
        var rawPacket = e.GetPacket();
        var packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

        var tcpPacket = packet.Extract<PacketDotNet.TcpPacket>();
        if (tcpPacket is null)
        {
            return;
        }

        if (!tcpPacket.HasPayloadData || tcpPacket.PayloadData.Length == 0)
        {
            return;
        }

        var ipPacket = (PacketDotNet.IPPacket)tcpPacket.ParentPacket;
        System.Net.IPAddress srcIp = ipPacket.SourceAddress;
        System.Net.IPAddress dstIp = ipPacket.DestinationAddress;
        int srcPort = tcpPacket.SourcePort;
        int dstPort = tcpPacket.DestinationPort;

        var tcpConnection = new TcpConnection(srcIp.Address, srcPort, dstIp.Address, dstPort);

        if (!_connections.ContainsKey(tcpConnection))
        {
            _connections.TryAdd
            (
                tcpConnection,
                new ConnectionData
                (
                    srcIp,
                    srcPort,
                    dstIp,
                    dstPort,
                    new List<byte[]>(),
                    DateTimeOffset.Now
                )
            );
        }

        var data = _connections[tcpConnection];
        data.LastReceivedAt = DateTimeOffset.Now;
        if (data.SniffedData.Count < 5 && tcpPacket.PayloadData.Length < 500
            && data.FirstObservedAt.AddMilliseconds(_options.CleanSniffedDataInterval) > DateTimeOffset.Now)
        {
            data.SniffedData.Add(tcpPacket.PayloadData);
        }

        if (_clients.TryGetValue(tcpConnection, out var clients))
        {
            foreach (var client in clients)
            {
                client.OnPacketArrival((LibPcapLiveDevice)e.Device, tcpConnection, tcpPacket.PayloadData);
            }
        }
    }

    private async Task DeletionTask(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                DeleteData();
                await Task.Delay(TimeSpan.FromMilliseconds(_options.CleanSniffedDataInterval * 3), ct);
            }
            catch (OperationCanceledException)
            {
                // ignored
            }
            catch (Exception e)
            {
                _logger.LogError(e, "The pcap manager deletion task has thrown an exception");
            }
        }
    }

    private void DeleteData()
    {
        foreach (var connectionData in _connections)
        {
            if (connectionData.Value.FirstObservedAt.AddMilliseconds
                    (_options.ForgetConnectionInterval) < DateTimeOffset.Now)
            {
                _connections.TryRemove(connectionData);
            }

            if (connectionData.Value.SniffedData.Count > 0 && connectionData.Value.LastReceivedAt.AddMilliseconds
                    (_options.CleanSniffedDataInterval) < DateTimeOffset.Now)
            {
                connectionData.Value.SniffedData.Clear();
            }
        }
    }
}