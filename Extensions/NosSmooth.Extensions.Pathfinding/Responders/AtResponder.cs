//
//  AtResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Packets.Server.Maps;
using Remora.Results;

namespace NosSmooth.Extensions.Pathfinding.Responders;

/// <inheritdoc />
internal class AtResponder : IPacketResponder<AtPacket>
{
    private readonly PathfinderState _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtResponder"/> class.
    /// </summary>
    /// <param name="state">The state.</param>
    public AtResponder(PathfinderState state)
    {
        _state = state;

    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<AtPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;

        _state.X = packet.X;
        _state.Y = packet.Y;

        return Task.FromResult(Result.FromSuccess());
    }
}