//
//  EquipPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Server.Weapons;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Inventory;

/// <summary>
/// Information about the player's equipped items.
/// </summary>
/// <param name="WeaponUpgradeRareSubPacket">The main weapon upgrade rare.</param>
/// <param name="ArmorUpgradeRareSubPacket">The armor upgrade rare.</param>
/// <param name="EquipSubPacket">The packet with equipment.</param>
[PacketHeader("equip", PacketSource.Server)]
[GenerateSerializer(true)]
public record EquipPacket
(
    [PacketIndex(0)]
    UpgradeRareSubPacket? WeaponUpgradeRareSubPacket,
    [PacketIndex(1)]
    UpgradeRareSubPacket? ArmorUpgradeRareSubPacket,
    [PacketListIndex(2, ListSeparator = ' ', InnerSeparator = '.')]
    IReadOnlyList<EquipSubPacket> EquipSubPacket
) : IPacket;