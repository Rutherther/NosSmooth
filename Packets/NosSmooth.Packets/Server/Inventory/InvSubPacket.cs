//
//  InvSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Inventory;

/// <summary>
/// A sub packet of <see cref="InvPacket"/>
/// containing items in the given bag.
/// </summary>
/// <param name="Slot">The slot the item is in.</param>
/// <param name="VNum">The vnum of the item.</param>
/// <param name="RareOrAmount">Rare for equipment, otherwise amount.</param>
/// <param name="UpgradeOrDesign">Upgrade for equipment, otherwise design. May not be present.</param>
/// <param name="SpStoneUpgrade">A stone upgrade of sp. Zero for other equipment. Not present otherwise.</param>
/// <param name="RuneCount">The rune count for equipment.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record InvSubPacket
(
    [PacketIndex(0)]
    short Slot,
    [PacketIndex(1)]
    short? VNum,
    [PacketIndex(2)]
    short RareOrAmount,
    [PacketIndex(3, IsOptional = true)]
    short? UpgradeOrDesign,
    [PacketIndex(4, IsOptional = true)]
    byte? SpStoneUpgrade,
    [PacketIndex(5, IsOptional = true)]
    int? RuneCount
) : IPacket;