//
//  StPacketResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;
using NosSmooth.Packets.Server.Entities;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Entities;

/// <summary>
/// Responds to st packet.
/// </summary>
public class StPacketResponder : IPacketResponder<StPacket>
{
    private readonly Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="StPacketResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    public StPacketResponder(Game game)
    {
        _game = game;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<StPacket> packetArgs, CancellationToken ct = default)
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

        entity.EffectsVNums = packet.BuffVNums?.Select(x => x.CardId).ToList();
        entity.Level = packet.Level;
        if (entity is Player player)
        {
            player.HeroLevel = packet.HeroLevel;
        }

        if (entity.Hp is null)
        {
            entity.Hp = new Health
            {
                Amount = packet.Hp,
                Percentage = packet.HpPercentage
            };
        }
        else
        {
            entity.Hp.Amount = packet.Hp;
            entity.Hp.Percentage = packet.HpPercentage;
        }

        if (entity.Mp is null)
        {
            entity.Mp = new Health
            {
                Amount = packet.Mp,
                Percentage = packet.MpPercentage
            };
        }
        else
        {
            entity.Mp.Amount = packet.Mp;
            entity.Mp.Percentage = packet.MpPercentage;
        }

        return Task.FromResult(Result.FromSuccess());
    }
}