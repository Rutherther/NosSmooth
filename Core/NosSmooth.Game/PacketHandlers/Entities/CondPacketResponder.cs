//
//  CondPacketResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Entities;
using NosSmooth.Packets.Server.Entities;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Entities;

/// <summary>
/// Responder to cond packet.
/// </summary>
public class CondPacketResponder : IPacketResponder<CondPacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="CondPacketResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    public CondPacketResponder(Game game, EventDispatcher eventDispatcher)
    {
        _game = game;
        _eventDispatcher = eventDispatcher;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<CondPacket> packetArgs, CancellationToken ct = default)
    {
        var map = _game.CurrentMap;
        if (map is null)
        {
            return Result.FromSuccess();
        }

        var packet = packetArgs.Packet;
        var entity = map.Entities.GetEntity<ILivingEntity>(packet.EntityId);

        if (entity is null)
        {
            return Result.FromSuccess();
        }

        bool cantMove = entity.CantMove;
        bool cantAttack = entity.CantAttack;

        entity.Speed = packet.Speed;
        entity.CantAttack = packet.CantAttack;
        entity.CantMove = packet.CantMove;

        if (cantMove != packet.CantMove || cantAttack != packet.CantAttack)
        {
            return await _eventDispatcher.DispatchEvent(new EntityStunnedEvent(entity, packet.CantMove, packet.CantAttack), ct);
        }

        return Result.FromSuccess();
    }
}