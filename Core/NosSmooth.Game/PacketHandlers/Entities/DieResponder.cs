//
//  DieResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Entities;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.Packets.Server.Entities;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Entities;

/// <summary>
/// A responder to <see cref="DiePacket"/>.
/// </summary>
public class DieResponder : IPacketResponder<DiePacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="DieResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    public DieResponder(Game game, EventDispatcher eventDispatcher)
    {
        _game = game;
        _eventDispatcher = eventDispatcher;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<DiePacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var entity = _game.CurrentMap?.Entities.GetEntity<ILivingEntity>(packet.TargetEntityId);

        if (entity is null)
        {
            return Task.FromResult(Result.FromSuccess());
        }

        if (entity.Type is not(EntityType.Player or EntityType.Npc))
        {
            _game.CurrentMap?.Entities.RemoveEntity(entity);
        }

        entity.Hp ??= new Health();
        entity.Hp.Amount = 0;
        entity.Hp.Percentage = 0;

        return _eventDispatcher.DispatchEvent(new EntityDiedEvent(entity, null), ct);
    }
}