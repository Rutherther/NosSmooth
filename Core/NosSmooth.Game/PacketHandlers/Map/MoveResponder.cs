//
//  MoveResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;
using NosSmooth.Packets.Server.Entities;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Map;

/// <summary>
/// Responds to move packet.
/// </summary>
public class MoveResponder : IPacketResponder<MovePacket>
{
    private readonly Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="MoveResponder"/> class.
    /// </summary>
    /// <param name="game">The nostale game.</param>
    public MoveResponder(Game game)
    {
        _game = game;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<MovePacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var map = _game.CurrentMap;

        // TODO: store entities somewhere else so we can store them even if the map is still null?
        if (map is null)
        {
            return Result.FromSuccess();
        }

        var entity = map.Entities.GetEntity<ILivingEntity>(packet.EntityId);
        if (entity is not null && entity.Position is not null)
        {
            entity.Position = new Position(packet.MapX, packet.MapY);
            entity.Speed = packet.Speed;
        }

        return Result.FromSuccess();
    }
}