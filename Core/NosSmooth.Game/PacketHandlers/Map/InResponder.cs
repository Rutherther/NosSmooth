//
//  InResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using NosSmooth.Core.Extensions;
using NosSmooth.Core.Packets;
using NosSmooth.Data.Abstractions;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Data.Social;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Entities;
using NosSmooth.Game.Helpers;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.Packets.Server.Maps;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Map;

/// <summary>
/// Responds to in packet.
/// </summary>
public class InResponder : IPacketResponder<InPacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;
    private readonly IInfoService _infoService;
    private readonly ILogger<InResponder> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="InResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    /// <param name="infoService">The info service.</param>
    /// <param name="logger">The logger.</param>
    public InResponder
    (
        Game game,
        EventDispatcher eventDispatcher,
        IInfoService infoService,
        ILogger<InResponder> logger
    )
    {
        _game = game;
        _eventDispatcher = eventDispatcher;
        _infoService = infoService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<InPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var map = _game.CurrentMap;
        if (map is null)
        {
            return Result.FromSuccess();
        }

        var entities = map.Entities;

        // add entity to the map
        var entity = await CreateEntityFromInPacket(packet, ct);
        entities.AddEntity(entity);

        return await _eventDispatcher.DispatchEvent(new EntityJoinedMapEvent(entity), ct);
    }

    private async Task<IEntity> CreateEntityFromInPacket(InPacket packet, CancellationToken ct)
    {
        if (packet.ItemSubPacket is not null)
        {
            return await CreateGroundItem(packet, packet.ItemSubPacket, ct);
        }
        if (packet.PlayerSubPacket is not null)
        {
            return await CreatePlayer(packet, packet.PlayerSubPacket, ct);
        }
        if (packet.NonPlayerSubPacket is not null)
        {
            if (packet.EntityType == EntityType.Npc)
            {
                return await CreateNpc(packet, packet.NonPlayerSubPacket, ct);
            }

            return await CreateMonster(packet, packet.NonPlayerSubPacket, ct);
        }

        throw new Exception("The in packet did not contain any subpacket. Bug?");
    }

    private async Task<GroundItem> CreateGroundItem
        (InPacket packet, InItemSubPacket itemSubPacket, CancellationToken ct)
    {
        if (packet.VNum is null)
        {
            throw new Exception("The vnum from the in packet cannot be null for items.");
        }
        var itemInfoResult = await _infoService.GetItemInfoAsync(packet.VNum.Value, ct);
        if (!itemInfoResult.IsDefined(out var itemInfo))
        {
            _logger.LogWarning
            (
                "Could not obtain an item info for vnum {vnum}: {error}",
                packet.VNum.Value,
                itemInfoResult.ToFullString()
            );
        }

        return new GroundItem
        {
            Amount = itemSubPacket.Amount,
            Id = packet.EntityId,
            OwnerId = itemSubPacket.OwnerId,
            IsQuestRelated = itemSubPacket.IsQuestRelative,
            ItemInfo = itemInfo,
            Position = new Position(packet.PositionX, packet.PositionY),
            VNum = packet.VNum.Value,
        };
    }

    private async Task<Player> CreatePlayer(InPacket packet, InPlayerSubPacket playerSubPacket, CancellationToken ct)
    {
        return new Player
        {
            Position = new Position(packet.PositionX, packet.PositionY),
            Id = packet.EntityId,
            Name = packet.Name?.Name,
            ArenaWinner = playerSubPacket.ArenaWinner,
            Class = playerSubPacket.Class,
            Compliment = playerSubPacket.Compliment,
            Direction = packet.Direction,
            Equipment = await EquipmentHelpers.CreateEquipmentFromInSubpacketAsync
            (
                _infoService,
                playerSubPacket.Equipment,
                playerSubPacket.WeaponUpgradeRareSubPacket,
                playerSubPacket.ArmorUpgradeRareSubPacket,
                ct
            ),
            Faction = playerSubPacket.Faction,
            Size = playerSubPacket.Size,
            Authority = playerSubPacket.Authority,
            Sex = playerSubPacket.Sex,
            HairStyle = playerSubPacket.HairStyle,
            HairColor = playerSubPacket.HairColor,
            Icon = playerSubPacket.ReputationIcon,
            IsInvisible = playerSubPacket.IsInvisible,
            Title = playerSubPacket.Title,
            Level = playerSubPacket.Level,
            HeroLevel = playerSubPacket.HeroLevel,
            Morph = new Morph(playerSubPacket.MorphVNum, playerSubPacket.MorphUpgrade),
            Family = playerSubPacket.FamilySubPacket.Value is null
                ? null
                : new Family
                (
                    playerSubPacket.FamilySubPacket.Value.FamilyId,
                    playerSubPacket.FamilySubPacket.Value.Title,
                    playerSubPacket.FamilyName,
                    playerSubPacket.Level,
                    playerSubPacket.FamilyIcons
                ),
        };
    }

    private async Task<Npc> CreateNpc
        (InPacket packet, InNonPlayerSubPacket nonPlayerSubPacket, CancellationToken ct)
    {
        if (packet.VNum is null)
        {
            throw new Exception("The vnum from the in packet cannot be null for monsters.");
        }

        var monsterInfoResult = await _infoService.GetMonsterInfoAsync(packet.VNum.Value, ct);
        if (!monsterInfoResult.IsDefined(out var monsterInfo))
        {
            _logger.LogWarning
            (
                "Could not obtain a monster info for vnum {vnum}: {error}",
                packet.VNum.Value,
                monsterInfoResult.ToFullString()
            );
        }

        return new Npc
        {
            VNum = packet.VNum.Value,
            NpcInfo = monsterInfo,
            Id = packet.EntityId,
            Direction = packet.Direction,
            Faction = nonPlayerSubPacket.Faction,
            Hp = new Health { Percentage = nonPlayerSubPacket.HpPercentage },
            Mp = new Health { Percentage = nonPlayerSubPacket.MpPercentage },
            Name = nonPlayerSubPacket.Name?.Name,
            Position = new Position(packet.PositionX, packet.PositionY),
            IsInvisible = nonPlayerSubPacket.IsInvisible,
            Level = monsterInfo?.Level ?? null,
            IsSitting = nonPlayerSubPacket.IsSitting,
            OwnerId = nonPlayerSubPacket.OwnerId,
            IsPartner = nonPlayerSubPacket.PartnerMask == 1,
            Skills = new Skill[]
            {
                await CreateSkill(nonPlayerSubPacket.Skill1, nonPlayerSubPacket.SkillRank1, ct),
                await CreateSkill(nonPlayerSubPacket.Skill2, nonPlayerSubPacket.SkillRank2, ct),
                await CreateSkill(nonPlayerSubPacket.Skill3, nonPlayerSubPacket.SkillRank3, ct),
            }
        };
    }

    private async Task<Monster> CreateMonster
        (InPacket packet, InNonPlayerSubPacket nonPlayerSubPacket, CancellationToken ct)
    {
        if (packet.VNum is null)
        {
            throw new Exception("The vnum from the in packet cannot be null for monsters.");
        }

        var monsterInfoResult = await _infoService.GetMonsterInfoAsync(packet.VNum.Value, ct);
        if (!monsterInfoResult.IsDefined(out var monsterInfo))
        {
            _logger.LogWarning
            (
                "Could not obtain a monster info for vnum {vnum}: {error}",
                packet.VNum.Value,
                monsterInfoResult.ToFullString()
            );
        }

        return new Monster
        {
            VNum = packet.VNum.Value,
            MonsterInfo = monsterInfo,
            Id = packet.EntityId,
            Direction = packet.Direction,
            Faction = nonPlayerSubPacket.Faction,
            Hp = new Health { Percentage = nonPlayerSubPacket.HpPercentage },
            Mp = new Health { Percentage = nonPlayerSubPacket.MpPercentage },
            Name = nonPlayerSubPacket.Name?.Name,
            Position = new Position(packet.PositionX, packet.PositionY),
            IsInvisible = nonPlayerSubPacket.IsInvisible,
            Level = monsterInfo?.Level ?? null,
            IsSitting = nonPlayerSubPacket.IsSitting,
            Skills = new Skill[]
            {
                await CreateSkill(nonPlayerSubPacket.Skill1, nonPlayerSubPacket.SkillRank1, ct),
                await CreateSkill(nonPlayerSubPacket.Skill2, nonPlayerSubPacket.SkillRank2, ct),
                await CreateSkill(nonPlayerSubPacket.Skill3, nonPlayerSubPacket.SkillRank3, ct),
            }
        };
    }

    private async Task<Skill> CreateSkill(int vnum, int? level, CancellationToken ct = default)
    {
        var skillInfoResult = await _infoService.GetSkillInfoAsync(vnum, ct);
        if (!skillInfoResult.IsSuccess)
        {
            _logger.LogWarning
            (
                "Could not obtain a skill info for vnum {vnum}: {error}",
                vnum,
                skillInfoResult.ToFullString()
            );
        }

        return new Skill(vnum, level, skillInfoResult.Entity);
    }
}