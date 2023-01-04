//
//  ScPPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Server.Character;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.Mates;

/// <summary>
/// Information about a pet the
/// character owns.
/// </summary>
/// <param name="PetId">The id of the pet entity.</param>
/// <param name="NpcVNum">The vnum of the pet.</param>
/// <param name="TransportId">Unknown TODO.</param>
/// <param name="Level">The level of the pet.</param>
/// <param name="Loyalty">The loyalty of the pet.</param>
/// <param name="Experience">The experience of the pet.</param>
/// <param name="Unknown1">Unknown TODO.</param>
/// <param name="AttackUpgrade">The upgrade of attack.</param>
/// <param name="MinimumAttack">The minimum attack amount.</param>
/// <param name="MaximumAttack">The maximum attack amount.</param>
/// <param name="Concentrate">Unknown TODO.</param>
/// <param name="CriticalChance">The critical chance of the pet's hit.</param>
/// <param name="CriticalRate">The critical rate of the pet's hit.</param>
/// <param name="DefenceUpgrade">The pet's defence upgrade.</param>
/// <param name="MeleeDefence">The melee defence.</param>
/// <param name="MeleeDefenceDodge">The dodge of melee defence.</param>
/// <param name="RangeDefence">The ranged defence.</param>
/// <param name="RangeDodgeRate">The dodge of ranged defence.</param>
/// <param name="MagicalDefence">The magical defence.</param>
/// <param name="Element">The element of the pet.</param>
/// <param name="ResistanceSubPacket">Information about pet's resistance</param>
/// <param name="Hp">The current hp of the pet.</param>
/// <param name="HpMax">The maximum hp of the pet.</param>
/// <param name="Mp">The current mp of the pet.</param>
/// <param name="MpMax">The maximum mp of the pet.</param>
/// <param name="Unknown4">Unknown TODO.</param>
/// <param name="LevelExperience">The maximum experience in current level of the pet.</param>
/// <param name="CanPickUp">Whether the pet can pick up stuff.</param>
/// <param name="Name">The name of the pet.</param>
/// <param name="IsSummonable">Whether the pet is summonable.</param>
[PacketHeader("sc_p", PacketSource.Server)]
[GenerateSerializer(true)]
public record ScPPacket
(
    [PacketIndex(0)]
    long PetId,
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
    [PacketIndex(6)]
    short Unknown1,
    [PacketIndex(7)]
    short AttackUpgrade,
    [PacketIndex(8)]
    int MinimumAttack,
    [PacketIndex(9)]
    int MaximumAttack,
    [PacketIndex(10)]
    int Concentrate,
    [PacketIndex(11)]
    int CriticalChance,
    [PacketIndex(12)]
    int CriticalRate,
    [PacketIndex(13)]
    short DefenceUpgrade,
    [PacketIndex(14)]
    int MeleeDefence,
    [PacketIndex(15)]
    int MeleeDefenceDodge,
    [PacketIndex(16)]
    int RangeDefence,
    [PacketIndex(17)]
    int RangeDodgeRate,
    [PacketIndex(18)]
    int MagicalDefence,
    [PacketIndex(19)]
    Element Element,
    [PacketIndex(20, InnerSeparator = ' ')]
    ResistanceSubPacket ResistanceSubPacket,
    [PacketIndex(21)]
    int Hp,
    [PacketIndex(22)]
    int HpMax,
    [PacketIndex(23)]
    int Mp,
    [PacketIndex(24)]
    int MpMax,
    [PacketIndex(25)]
    int Unknown4,
    [PacketIndex(26)]
    int LevelExperience,
    [PacketIndex(27)]
    bool CanPickUp,
    [PacketIndex(28)]
    NameString Name,
    [PacketIndex(29)]
    bool IsSummonable
) : IPacket;