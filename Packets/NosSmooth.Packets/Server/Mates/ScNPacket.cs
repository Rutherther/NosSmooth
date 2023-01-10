//
//  ScNPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Server.Character;
using NosSmooth.Packets.Server.Maps;
using NosSmooth.PacketSerializer.Abstractions;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.Mates;

/// <summary>
/// Information about a partner the
/// character owns.
/// </summary>
/// <param name="PartnerId">The id of the partner entity.</param>
/// <param name="NpcVNum">The vnum of the partner.</param>
/// <param name="TransportId">Unknown TODO.</param>
/// <param name="Level">The level of the partner.</param>
/// <param name="Loyalty">The loyalty of the partner.</param>
/// <param name="Experience">The experience of the partner.</param>
/// <param name="WeaponSubPacket">Information about partner's weapon.</param>
/// <param name="ArmorSubPacket">Information about partner's armor.</param>
/// <param name="GauntletSubPacket">Information about partner's gauntlet.</param>
/// <param name="BootsSubPacket">Information about partner's boots.</param>
/// <param name="Unknown1">Unknown TODO.</param>
/// <param name="Unknown2">Unknown TODO.</param>
/// <param name="Unknown3">Unknown TODO.</param>
/// <param name="AttackUpgrade">The upgrade of attack.</param>
/// <param name="MinimumAttack">The minimum attack amount.</param>
/// <param name="MaximumAttack">The maximum attack amount.</param>
/// <param name="Precision">Unknown TODO.</param>
/// <param name="CriticalChance">The critical chance of the partner's hit.</param>
/// <param name="CriticalRate">The critical rate of the partner's hit.</param>
/// <param name="DefenceUpgrade">The partner's defence upgrade.</param>
/// <param name="MeleeDefence">The melee defence.</param>
/// <param name="MeleeDefenceDodge">The dodge of melee defence.</param>
/// <param name="RangeDefence">The ranged defence.</param>
/// <param name="RangeDodgeRate">The dodge of ranged defence.</param>
/// <param name="MagicalDefence">The magical defence.</param>
/// <param name="Element">The element of the partner.</param>
/// <param name="ResistanceSubPacket">Information about partner's resistance</param>
/// <param name="Hp">The current hp of the partner.</param>
/// <param name="HpMax">The maximum hp of the partner.</param>
/// <param name="Mp">The current mp of the partner.</param>
/// <param name="MpMax">The maximum mp of the partner.</param>
/// <param name="Unknown4">Unknown TODO.</param>
/// <param name="LevelExperience">The maximum experience in current level of the partner.</param>
/// <param name="Name">The name of the partner.</param>
/// <param name="MorphVNum">The morph vnum of the partner, if any.</param>
/// <param name="IsSummonable">Whether the partner is summonable.</param>
/// <param name="SpSubPacket">The currently equipped sp of the partner.</param>
/// <param name="Skill1SubPacket">Information about first skill of the partner's sp.</param>
/// <param name="Skill2SubPacket">Information about second skill of the partner's sp.</param>
/// <param name="Skill3SubPacket">Information about third skill of the partner's sp.</param>
[PacketHeader("sc_n", PacketSource.Server)]
[GenerateSerializer(true)]
public record ScNPacket
(
    [PacketIndex(0)]
    long PartnerId,
    [PacketIndex(1)]
    long NpcVNum,
    [PacketIndex(2)]
    long TransportId,
    [PacketIndex(3)]
    short Level,
    [PacketIndex(4)]
    short Loyalty,
    [PacketIndex(5)]
    long Experience,
    [PacketIndex(6, InnerSeparator = '.')]
    NullableWrapper<ScNEquipmentSubPacket> WeaponSubPacket,
    [PacketIndex(7, InnerSeparator = '.')]
    NullableWrapper<ScNEquipmentSubPacket> ArmorSubPacket,
    [PacketIndex(8, InnerSeparator = '.')]
    NullableWrapper<ScNEquipmentSubPacket> GauntletSubPacket,
    [PacketIndex(9, InnerSeparator = '.')]
    NullableWrapper<ScNEquipmentSubPacket> BootsSubPacket,
    [PacketIndex(10, InnerSeparator = '.')]
    short Unknown1,
    [PacketIndex(11)]
    short Unknown2,
    [PacketIndex(12)]
    short AttackType,
    [PacketIndex(13)]
    short AttackUpgrade,
    [PacketIndex(14)]
    int MinimumAttack,
    [PacketIndex(15)]
    int MaximumAttack,
    [PacketIndex(16)]
    int Precision,
    [PacketIndex(17)]
    int CriticalChance,
    [PacketIndex(18)]
    int CriticalRate,
    [PacketIndex(19)]
    short DefenceUpgrade,
    [PacketIndex(20)]
    int MeleeDefence,
    [PacketIndex(21)]
    int MeleeDefenceDodge,
    [PacketIndex(22)]
    int RangeDefence,
    [PacketIndex(23)]
    int RangeDodgeRate,
    [PacketIndex(24)]
    int MagicalDefence,
    [PacketIndex(25)]
    Element Element,
    [PacketIndex(26, InnerSeparator = ' ')]
    ResistanceSubPacket ResistanceSubPacket,
    [PacketIndex(27)]
    int Hp,
    [PacketIndex(28)]
    int HpMax,
    [PacketIndex(29)]
    int Mp,
    [PacketIndex(30)]
    int MpMax,
    [PacketIndex(31)]
    bool IsTeamMember,
    [PacketIndex(32)]
    int LevelExperience,
    [PacketIndex(33)]
    NameString Name,
    [PacketIndex(34)]
    int? MorphVNum,
    [PacketIndex(35)]
    bool IsSummonable,
    [PacketIndex(36, InnerSeparator = '.')]
    NullableWrapper<ScNSpSubPacket> SpSubPacket,
    [PacketIndex(37, InnerSeparator = '.')]
    NullableWrapper<ScNSkillSubPacket> Skill1SubPacket,
    [PacketIndex(38, InnerSeparator = '.')]
    NullableWrapper<ScNSkillSubPacket> Skill2SubPacket,
    [PacketIndex(39, InnerSeparator = '.')]
    NullableWrapper<ScNSkillSubPacket> Skill3SubPacket
) : IPacket;