//
//  PcapNostaleManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using SharpPcap;
using SharpPcap.LibPcap;

namespace NosSmooth.Pcap;

/// <summary>
/// Captures packets, distributes them to Pcap clients.
/// </summary>
public class PcapNostaleManager
{
    private readonly ConcurrentDictionary<TcpConnection, ConnectionData> _connections;
    private readonly ConcurrentDictionary<TcpConnection, PcapNostaleClient> _clients;
    private int _clientsCount;
    private bool _started;

    /// <summary>
    /// Initializes a new instance of the <see cref="PcapNostaleManager"/> class.
    /// </summary>
    public PcapNostaleManager()
    {
        _connections = new ConcurrentDictionary<TcpConnection, ConnectionData>();
        _clients = new ConcurrentDictionary<TcpConnection, PcapNostaleClient>();
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
        _clients.AddOrUpdate(connection, (c) => client, (c1, c2) => client);

        if (_connections.TryGetValue(connection, out var data))
        {
            foreach (var sniffedPacket in data.SniffedData)
            {
                client.OnPacketArrival(connection, sniffedPacket);
            }
        }
    }

    /// <summary>
    /// Disassociate the given connection.
    /// </summary>
    /// <param name="connection">The connection to disassociate.</param>
    internal void UnregisterConnection(TcpConnection connection)
    {
        _clients.TryRemove(connection, out _);
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
        var rawPacket = e.GetPacket();

        var packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

        var tcpPacket = packet.Extract<PacketDotNet.TcpPacket>();
        if (tcpPacket is null)
        {
            return;
        }

        if (!tcpPacket.HasPayloadData || tcpPacket.PayloadData.Length == 0 || tcpPacket.PayloadData.Length > 500)
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
        if (data.SniffedData.Count < 5)
        {
            data.SniffedData.Add(tcpPacket.PayloadData);
        } // TODO: clean up the sniffed data in case they are not needed.

        if (_clients.TryGetValue(tcpConnection, out var client))
        {
            client.OnPacketArrival(tcpConnection, tcpPacket.PayloadData);
        }
    }
}