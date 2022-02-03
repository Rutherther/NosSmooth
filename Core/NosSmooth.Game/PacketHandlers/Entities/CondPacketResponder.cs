//
//  CondPacketResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Packets.Server.Entities;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Entities;

/// <summary>
/// Responder to cond packet.
/// </summary>
public class CondPacketResponder : IPacketResponder<CondPacket>
{
    private readonly Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="CondPacketResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    public CondPacketResponder(Game game)
    {
        _game = game;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<CondPacket> packetArgs, CancellationToken ct = default)
    {
        var map = _game.CurrentMap;
        if (map is null)
        {
            return Task.FromResult(Result.FromSuccess());
        }

        var packet = packetArgs.Packet;
        var entity = map.Entities.GetEntity<ILivingEntity>(packet.EntityId);

        if (entity is null)
        {
            return Task.FromResult(Result.FromSuccess());
        }

        entity.Speed = packet.Speed;
        entity.CantAttack = packet.CantAttack;
        entity.CantMove = packet.CantMove;

        return Task.FromResult(Result.FromSuccess());
    }
}