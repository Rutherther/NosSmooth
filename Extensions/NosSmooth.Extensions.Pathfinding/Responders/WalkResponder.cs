//
//  WalkResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Packets.Client.Movement;
using Remora.Results;

namespace NosSmooth.Extensions.Pathfinding.Responders;

/// <inheritdoc />
internal class WalkResponder : IPacketResponder<WalkPacket>
{
    private readonly PathfinderState _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="WalkResponder"/> class.
    /// </summary>
    /// <param name="state">The state.</param>
    public WalkResponder(PathfinderState state)
    {
        _state = state;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<WalkPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;

        _state.X = packet.PositionX;
        _state.Y = packet.PositionY;

        return Task.FromResult(Result.FromSuccess());
    }
}