//
//  InPlayerSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using NosSmooth.Packets.Attributes;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Players;
using NosSmooth.Packets.Packets.Server.Players;
using NosSmooth.Packets.Packets.Server.Weapons;

namespace NosSmooth.Packets.Packets.Server.Entities;

[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer]
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
    byte Unknown3,
    [PacketIndex(16)]
    UpgradeRareSubPacket WeaponUpgradeRareSubPacket,
    [PacketIndex(17)]
    UpgradeRareSubPacket ArmorUpgradeRareSubPacket,
    [PacketIndex(18)]
    FamilySubPacket FamilySubPacket,
    [PacketIndex(19)]
    string ReputationIcon,
    [PacketIndex(20)]
    bool IsInvisible,
    [PacketIndex(21)]
    byte MorphUpgrade,
    [PacketIndex(22)]
    FactionType Faction,
    [PacketIndex(23)]
    byte MorphUpgrade2,
    [PacketIndex(24)]
    byte Level,
    [PacketIndex(25)]
    byte FamilyLevel,
    [PacketListIndex(26, ListSeparator = '|')]
    IReadOnlyList<bool> FamilyIcons,
    [PacketIndex(27)]
    bool ArenaWinner,
    [PacketIndex(28)]
    short Compliment,
    [PacketIndex(29)]
    byte Size,
    [PacketIndex(30)]
    byte HeroLevel,
    [PacketIndex(31)]
    short Title
) : IPacket;