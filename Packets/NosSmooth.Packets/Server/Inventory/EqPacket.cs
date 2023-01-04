//
//  EqPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Players;
using NosSmooth.Packets.Server.Maps;
using NosSmooth.Packets.Server.Weapons;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Inventory;

/// <summary>
/// Player
/// </summary>
/// <param name="CharacterId"></param>
/// <param name="AuthorityType"></param>
/// <param name="Sex"></param>
/// <param name="HairStyle"></param>
/// <param name="HairColor"></param>
/// <param name="Class"></param>
/// <param name="EquipmentSubPacket"></param>
/// <param name="WeaponUpgradeRareSubPacket"></param>
/// <param name="ArmorUpgradeRareSubPacket"></param>
/// <param name="Size"></param>
[PacketHeader("eq", PacketSource.Server)]
[GenerateSerializer(true)]
public record EqPacket
(
    [PacketIndex(0)]
    long CharacterId,
    [PacketIndex(1)]
    AuthorityType AuthorityType,
    [PacketIndex(2)]
    SexType Sex,
    [PacketIndex(3)]
    HairStyle HairStyle,
    [PacketIndex(4)]
    HairColor HairColor,
    [PacketIndex(5)]
    PlayerClass Class,
    [PacketIndex(6, InnerSeparator = '.')]
    InEquipmentSubPacket EquipmentSubPacket,
    [PacketIndex(7)]
    UpgradeRareSubPacket? WeaponUpgradeRareSubPacket,
    [PacketIndex(8)]
    UpgradeRareSubPacket? ArmorUpgradeRareSubPacket,
    [PacketIndex(9, IsOptional = true)]
    byte? Size
) : IPacket;