//
//  MatesInitResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using NosSmooth.Core.Extensions;
using NosSmooth.Core.Packets;
using NosSmooth.Data.Abstractions;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Data.Items;
using NosSmooth.Game.Data.Mates;
using NosSmooth.Game.Data.Social;
using NosSmooth.Game.Data.Stats;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Mates;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.Packets.Enums.Mates;
using NosSmooth.Packets.Server.Groups;
using NosSmooth.Packets.Server.Mates;
using NosSmooth.Packets.Server.Miniland;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Relations;

/// <summary>
/// A mates initialization responder.
/// </summary>
public class MatesInitResponder : IPacketResponder<ScPPacket>, IPacketResponder<ScNPacket>,
    IPacketResponder<PinitPacket>, IPacketResponder<PstPacket>, IPacketResponder<PClearPacket>
{
    private readonly Game _game;
    private readonly IInfoService _infoService;
    private readonly EventDispatcher _eventDispatcher;
    private readonly ILogger<MatesInitResponder> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MatesInitResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="infoService">The info service.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    /// <param name="logger">The logger.</param>
    public MatesInitResponder
    (
        Game game,
        IInfoService infoService,
        EventDispatcher eventDispatcher,
        ILogger<MatesInitResponder> logger
    )
    {
        _game = game;
        _infoService = infoService;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<ScPPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var pet = new Pet
        (
            packet.PetId,
            packet.NpcVNum,
            packet.TransportId,
            new Level(packet.Level, packet.Experience, packet.LevelExperience),
            packet.Loyalty,
            new MateAttackStats
            (
                packet.AttackUpgrade,
                packet.MinimumAttack,
                packet.MaximumAttack,
                packet.Concentrate,
                packet.CriticalChance,
                packet.CriticalRate
            ),
            new MateArmorStats
            (
                packet.DefenceUpgrade,
                packet.MeleeDefence,
                packet.MeleeDefenceDodge,
                packet.RangeDefence,
                packet.RangeDodgeRate,
                packet.MagicalDefence
            ),
            packet.Element,
            new Resistance
            (
                packet.ResistanceSubPacket.FireResistance,
                packet.ResistanceSubPacket.WaterResistance,
                packet.ResistanceSubPacket.LightResistance,
                packet.ResistanceSubPacket.DarkResistance
            ),
            new Health { Amount = packet.Hp, Maximum = packet.HpMax },
            new Health { Amount = packet.Mp, Maximum = packet.MpMax },
            packet.Name,
            packet.IsSummonable,
            packet.CanPickUp
        );

        await _game.CreateOrUpdateMatesAsync
        (
            () =>
            {
                var mates = new Mates();
                mates.SetPet(pet);
                return mates;
            },
            mates =>
            {
                mates.SetPet(pet);
                return mates;
            },
            ct: ct
        );

        return await _eventDispatcher.DispatchEvent
        (
            new PetInitializedEvent(pet),
            ct
        );
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<ScNPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var partner = new Partner
        (
            packet.PartnerId,
            packet.NpcVNum,
            packet.TransportId,
            new Level(packet.Level, packet.Experience, packet.LevelExperience),
            packet.Loyalty,
            new MateAttackStats
            (
                packet.AttackUpgrade,
                packet.MinimumAttack,
                packet.MaximumAttack,
                packet.Precision,
                packet.CriticalChance,
                packet.CriticalRate
            ),
            new MateArmorStats
            (
                packet.DefenceUpgrade,
                packet.MeleeDefence,
                packet.MeleeDefenceDodge,
                packet.RangeDefence,
                packet.RangeDodgeRate,
                packet.MagicalDefence
            ),
            new PartnerEquipment
            (
                await CreatePartnerItem(packet.WeaponSubPacket, ct),
                await CreatePartnerItem(packet.ArmorSubPacket, ct),
                await CreatePartnerItem(packet.GauntletSubPacket, ct),
                await CreatePartnerItem(packet.BootsSubPacket, ct)
            ),
            packet.Element,
            new Resistance
            (
                packet.ResistanceSubPacket.FireResistance,
                packet.ResistanceSubPacket.WaterResistance,
                packet.ResistanceSubPacket.LightResistance,
                packet.ResistanceSubPacket.DarkResistance
            ),
            new Health { Amount = packet.Hp, Maximum = packet.HpMax },
            new Health { Amount = packet.Mp, Maximum = packet.MpMax },
            packet.Name,
            packet.MorphVNum,
            packet.IsSummonable,
#pragma warning disable SA1118
            packet.SpSubPacket?.ItemVNum is not null
                ? new PartnerSp
                (
                    packet.SpSubPacket.ItemVNum.Value,
                    packet.SpSubPacket.AgilityPercentage,
                    await CreateSkill(packet.Skill1SubPacket, ct),
                    await CreateSkill(packet.Skill2SubPacket, ct),
                    await CreateSkill(packet.Skill3SubPacket, ct)
                )
                : null
#pragma warning restore SA1118
        );

        await _game.CreateOrUpdateMatesAsync
        (
            () =>
            {
                var mates = new Mates();
                mates.SetPartner(partner);
                return mates;
            },
            mates =>
            {
                mates.SetPartner(partner);
                return mates;
            },
            ct: ct
        );

        return await _eventDispatcher.DispatchEvent
        (
            new PartnerInitializedEvent(partner),
            ct
        );
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<PinitPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var partner = packet.PinitSubPackets?.FirstOrDefault(x => x.MateSubPacket?.MateType == MateType.Partner);
        var pet = packet.PinitSubPackets?.FirstOrDefault(x => x.MateSubPacket?.MateType == MateType.Pet);

        Partner? gamePartner = null;
        Pet? gamePet = null;

        await _game.CreateOrUpdateMatesAsync
        (
            () => null,
            m =>
            {
                if (partner is not null)
                {
                    gamePartner = _game.Mates?.Partners.FirstOrDefault(x => x.MateId == partner.EntityId);
                    if (gamePartner is not null && gamePartner != m.CurrentPartner?.Partner)
                    {
                        m.CurrentPartner = new PartyPartner(gamePartner);
                    }
                }

                if (pet is not null)
                {
                    gamePet = _game.Mates?.Pets.FirstOrDefault(x => x.MateId == pet.EntityId);
                    if (gamePet is not null && gamePet != m.CurrentPet?.Pet)
                    {
                        m.CurrentPet = new PartyPet(gamePet);
                    }
                }

                return m;
            },
            ct: ct
        );

        return await _eventDispatcher.DispatchEvent
        (
            new MatesPartyInitializedEvent(gamePet, gamePartner),
            ct
        );
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<PstPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        if (packet.EntityType != EntityType.Npc || packet.MateType is null)
        {
            return Result.FromSuccess();
        }

        Mate? mate = null;
        var hp = new Health { Amount = packet.Hp, Percentage = packet.HpPercentage };
        var mp = new Health { Amount = packet.Mp, Percentage = packet.MpPercentage };

        await _game.CreateOrUpdateMatesAsync
        (
            () => null,
            m =>
            {
                if (packet.MateType is MateType.Pet && m.CurrentPet is not null)
                {
                    mate = m.CurrentPet.Pet;
                    m.CurrentPet.Hp = hp;
                    m.CurrentPet.Mp = mp;
                }
                else if (packet.MateType is MateType.Partner && m.CurrentPartner is not null)
                {
                    mate = m.CurrentPartner.Partner;
                    m.CurrentPartner.Hp = hp;
                    m.CurrentPartner.Mp = mp;
                }

                return m;
            },
            ct: ct
        );

        if (mate is not null)
        {
            return await _eventDispatcher.DispatchEvent(new MateStatEvent(mate, hp, mp), ct);
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<PClearPacket> packetArgs, CancellationToken ct = default)
    {
        await _game.CreateOrUpdateMatesAsync
        (
            () => null,
            m =>
            {
                m.Clear();
                return m;
            },
            ct: ct
        );

        return Result.FromSuccess();
    }

    private async Task<UpgradeableItem?> CreatePartnerItem(ScNEquipmentSubPacket? packet, CancellationToken ct)
    {
        if (packet is null || packet.ItemVNum is null)
        {
            return null;
        }

        var itemInfoResult = await _infoService.GetItemInfoAsync(packet.ItemVNum.Value, ct);
        if (!itemInfoResult.IsDefined(out var itemInfo))
        {
            _logger.LogWarning
            (
                "Could not obtain an item info for vnum {vnum}: {error}",
                packet.ItemVNum.Value,
                itemInfoResult.ToFullString()
            );
        }

        return new UpgradeableItem
        (
            packet.ItemVNum.Value,
            itemInfo,
            packet.ItemUpgrade,
            packet.ItemRare,
            0
        );
    }

    private async Task<PartnerSkill?> CreateSkill(ScNSkillSubPacket? packet, CancellationToken ct)
    {
        if (packet is null || packet.SkillVNum is null)
        {
            return null;
        }

        var skillInfoResult = await _infoService.GetSkillInfoAsync(packet.SkillVNum.Value, ct);
        if (!skillInfoResult.IsDefined(out var skillInfo))
        {
            _logger.LogWarning
            (
                "Could not obtain a skill info for vnum {vnum}: {error}",
                packet.SkillVNum.Value,
                skillInfoResult.ToFullString()
            );
        }

        return new PartnerSkill(packet.SkillVNum.Value, packet.Rank, skillInfo);
    }
}