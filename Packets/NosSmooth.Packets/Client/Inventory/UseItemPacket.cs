//
//  UseItemPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Inventory;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client.Inventory;

/// <summary>
/// Use an item from inventory.
/// </summary>
/// <param name="BagType">The type of the bag.</param>
/// <param name="Slot">The slot in the bag.</param>
[PacketHeader("u_i", PacketSource.Client)]
[GenerateSerializer(true)]
public record UseItemPacket
(
    [PacketIndex(0)]
    BagType BagType,
    [PacketIndex(1)]
    short Slot
) : IPacket;