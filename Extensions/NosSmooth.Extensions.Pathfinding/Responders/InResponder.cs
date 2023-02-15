//
//  InResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Packets.Client.Battle;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.Packets.Server.Maps;
using Remora.Results;

namespace NosSmooth.Extensions.Pathfinding.Responders;

/// <inheritdoc />
public class InResponder : IPacketResponder<InPacket>
{
    private readonly PathfinderState _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="InResponder"/> class.
    /// </summary>
    /// <param name="state">The pathfinder state.</param>
    public InResponder(PathfinderState state)
    {
        _state = state;

    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<InPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;

        if (packet.EntityType != EntityType.Npc)
        {
            return Task.FromResult(Result.FromSuccess());
        }

        if (packet.NonPlayerSubPacket is null)
        {
            return Task.FromResult(Result.FromSuccess());
        }

        if (packet.NonPlayerSubPacket.OwnerId != _state.Character.Id)
        {
            return Task.FromResult(Result.FromSuccess());
        }

        _state.AddEntity(packet.EntityId, packet.PositionX, packet.PositionY);
        return Task.FromResult(Result.FromSuccess());
    }
}