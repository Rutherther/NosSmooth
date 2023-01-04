//
//  EquipSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Inventory;

/// <summary>
/// A sub packet for <see cref="EquipPacket"/>
/// containing information about individual items equipped.
/// </summary>
/// <param name="Slot">The equipment slot.</param>
/// <param name="ItemVNum">The vnum of the item.</param>
/// <param name="Rare">The rare.</param>
/// <param name="DesignOrRare">Can be design for colored items, otherwise upgrade.</param>
/// <param name="Unknown">Unknown TODO. Seems to be always 0.</param>
/// <param name="RuneCount">The count of runes.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record EquipSubPacket
(
    [PacketIndex(0)]
    byte Slot,
    [PacketIndex(1)]
    long ItemVNum,
    [PacketIndex(2)]
    byte? Rare,
    [PacketIndex(3)]
    short DesignOrRare,
    [PacketIndex(4)]
    byte Unknown,
    [PacketIndex(5)]
    int RuneCount
) : IPacket;