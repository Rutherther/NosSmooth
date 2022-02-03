//
//  OutResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Maps;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Entities;
using NosSmooth.Game.Helpers;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Server.Maps;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Map;

/// <summary>
/// Responds to out packet.
/// </summary>
public class OutResponder : IPacketResponder<OutPacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    public OutResponder(Game game, EventDispatcher eventDispatcher)
    {
        _game = game;
        _eventDispatcher = eventDispatcher;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<OutPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var map = _game.CurrentMap;
        if (map is null)
        {
            return Result.FromSuccess();
        }
        map.Entities.RemoveEntity(packet.EntityId);
        Portal? portal = null;

        IEntity entity = map.Entities.GetEntity(packet.EntityId) ?? EntityHelpers.CreateEntity
            (packet.EntityType, packet.EntityId);

        var position = entity.Position;
        if (position is not null)
        {
            map.IsOnPortal(position.Value, out portal);
        }

        bool? died = null;
        if (entity is ILivingEntity livingEntity)
        {
            died = (livingEntity.Hp?.Amount ?? -1) == 0 || (livingEntity.Hp?.Percentage ?? -1) == 0;
        }

        return await _eventDispatcher.DispatchEvent(new EntityLeftMapEvent(entity, portal, died), ct);
    }
}