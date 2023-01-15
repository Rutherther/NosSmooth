//
//  InPlayerSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.Packets.Enums.Players;
using NosSmooth.Packets.Server.Character;
using NosSmooth.Packets.Server.Weapons;
using NosSmooth.PacketSerializer.Abstractions;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Maps;

/// <summary>
/// Sub packet for <see cref="InPacket"/>
/// sent for players.
/// </summary>
/// <param name="Authority">The authority of the player.</param>
/// <param name="Sex">The sex of the player.</param>
/// <param name="HairStyle">The hair style of the player.</param>
/// <param name="HairColor">The hair color of the player.</param>
/// <param name="Class">The class of the player.</param>
/// <param name="Equipment">The equipment sub packet of the player containing vnums of his equipment data.</param>
/// <param name="HpPercentage">The percentage of hp.</param>
/// <param name="MpPercentage">The percentage of mp.</param>
/// <param name="IsSitting">Whether the player is sitting.</param>
/// <param name="GroupId">What group does the player belong to.</param>
/// <param name="Fairy">The vnum of the fairy the player has.</param>
/// <param name="FairyElement">The element of the fairy.</param>
/// <param name="Unknown">Unknown TODO.</param>
/// <param name="MorphVNum">The vnum of the morph (used for special cards and such).</param>
/// <param name="Unknown2">Unknown TODO</param>
/// <param name="Unknown3">Unknown TODO</param>
/// <param name="WeaponUpgradeRareSubPacket">Weapon upgrade and rare sub packet.</param>
/// <param name="ArmorUpgradeRareSubPacket">Armor upgrade and rare sub packet.</param>
/// <param name="FamilySubPacket">Family information sub packet.</param>
/// <param name="FamilyName">The name of the family.</param>
/// <param name="ReputationIcon">The reputation icon number.</param>
/// <param name="IsInvisible">Whether the player is invisible.</param>
/// <param name="MorphUpgrade">The upgrade of the morph. (wings)</param>
/// <param name="Faction">The faction the player belongs to.</param>
/// <param name="MorphUpgrade2">Unknown TODO.</param>
/// <param name="Level">The level of the player.</param>
/// <param name="FamilyLevel">The level of the family the player belongs to.</param>
/// <param name="FamilyIcons">The family icons list.</param>
/// <param name="ArenaWinner">Whether the player is an arena winner.</param>
/// <param name="Compliment">Unknown TODO</param>
/// <param name="Size">The size of the player.</param>
/// <param name="HeroLevel">The hero level of the player.</param>
/// <param name="Title">The title of the player.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record InPlayerSubPacket
(
    [PacketIndex(0)]
    AuthorityType Authority,
    [PacketIndex(1)]
    SexType Sex,
    [PacketIndex(2)]
    HairStyle HairStyle,
    [PacketIndex(3)]
    HairColor HairColor,
    [PacketIndex(4)]
    PlayerClass Class,
    [PacketIndex(5, InnerSeparator = '.')]
    InEquipmentSubPacket Equipment,
    [PacketIndex(6)]
    short HpPercentage,
    [PacketIndex(7)]
    short MpPercentage,
    [PacketIndex(8)]
    bool IsSitting,
    [PacketIndex(9)]
    long? GroupId,
    [PacketIndex(10)]
    short Fairy,
    [PacketIndex(11)]
    Element FairyElement,
    [PacketIndex(12)]
    byte Unknown,
    [PacketIndex(13)]
    long MorphVNum,
    [PacketIndex(14)]
    byte Unknown2,
    [PacketIndex(15)]
    short Unknown3,
    [PacketIndex(16)]
    UpgradeRareSubPacket? WeaponUpgradeRareSubPacket,
    [PacketIndex(17)]
    UpgradeRareSubPacket? ArmorUpgradeRareSubPacket,
    [PacketIndex(18, InnerSeparator = '.')]
    NullableWrapper<FamilySubPacket> FamilySubPacket,
    [PacketIndex(19)]
    string? FamilyName,
    [PacketIndex(20)]
    short ReputationIcon,
    [PacketIndex(21)]
    bool IsInvisible,
    [PacketIndex(22)]
    byte MorphUpgrade,
    [PacketIndex(23)]
    FactionType Faction,
    [PacketIndex(24)]
    byte MorphUpgrade2,
    [PacketIndex(25)]
    byte Level,
    [PacketIndex(26)]
    byte FamilyLevel,
    [PacketListIndex(27, ListSeparator = '|')]
    IReadOnlyList<bool> FamilyIcons,
    [PacketIndex(28)]
    bool ArenaWinner,
    [PacketIndex(29)]
    short Compliment,
    [PacketIndex(30)]
    byte Size,
    [PacketIndex(31)]
    byte HeroLevel,
    [PacketIndex(32)]
    short Title
) : IPacket;