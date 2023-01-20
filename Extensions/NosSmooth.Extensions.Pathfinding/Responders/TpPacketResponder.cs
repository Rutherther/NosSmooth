//
//  TpPacketResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Packets.Server.Maps;
using Remora.Results;

namespace NosSmooth.Extensions.Pathfinding.Responders;

/// <inheritdoc />
public class TpPacketResponder : IPacketResponder<TpPacket>
{
    private readonly PathfinderState _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="TpPacketResponder"/> class.
    /// </summary>
    /// <param name="state">The state.</param>
    public TpPacketResponder(PathfinderState state)
    {
        _state = state;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<TpPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        if (packet.EntityId == _state.CharacterId)
        {
            _state.X = packet.PositionX;
            _state.Y = packet.PositionY;
        }

        return Task.FromResult(Result.FromSuccess());
    }
}