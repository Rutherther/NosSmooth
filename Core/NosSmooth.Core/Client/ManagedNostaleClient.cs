//
//  ManagedNostaleClient.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Commands;
using NosSmooth.Packets;
using NosSmooth.PacketSerializer;
using Remora.Results;

namespace NosSmooth.Core.Client;

/// <summary>
/// A NosTale client that supports sending and receiving packets using .
/// </summary>
public class ManagedNostaleClient : INostaleClient
{
    private readonly INostaleClient _rawClient;
    private readonly IPacketSerializer _packetSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManagedNostaleClient"/> class.
    /// </summary>
    /// <param name="rawClient">The raw nostale client.</param>
    /// <param name="packetSerializer">The packet serializer.</param>
    protected ManagedNostaleClient
    (
        INostaleClient rawClient,
        IPacketSerializer packetSerializer
    )
    {
        _rawClient = rawClient;
        _packetSerializer = packetSerializer;
    }

    /// <summary>
    /// Receives the given packet.
    /// </summary>
    /// <param name="packet">The packet to receive.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> ReceivePacketAsync(IPacket packet, CancellationToken ct = default)
    {
        var serialized = _packetSerializer.Serialize(packet);

        return serialized.IsSuccess
            ? ReceivePacketAsync(serialized.Entity, ct)
            : Task.FromResult(Result.FromError(serialized));
    }

    /// <summary>
    /// Sends the given packet to the server.
    /// </summary>
    /// <param name="packet">The packet to send.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> SendPacketAsync(IPacket packet, CancellationToken ct = default)
    {
        var serialized = _packetSerializer.Serialize(packet);

        return serialized.IsSuccess
            ? SendPacketAsync(serialized.Entity, ct)
            : Task.FromResult(Result.FromError(serialized));
    }

    /// <inheritdoc />
    public Task<Result> RunAsync(CancellationToken stopRequested = default)
        => _rawClient.RunAsync(stopRequested);

    /// <inheritdoc />
    public Task<Result> SendPacketAsync(string packetString, CancellationToken ct = default)
        => _rawClient.SendPacketAsync(packetString, ct);

    /// <inheritdoc />
    public Task<Result> ReceivePacketAsync(string packetString, CancellationToken ct = default)
        => _rawClient.ReceivePacketAsync(packetString, ct);

    /// <inheritdoc />
    public Task<Result> SendCommandAsync(ICommand command, CancellationToken ct = default)
        => _rawClient.SendCommandAsync(command, ct);
}