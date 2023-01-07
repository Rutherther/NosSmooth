//
//  MvePacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Inventory;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client.Inventory;

/// <summary>
/// Move item from one bag to another (ie. from main to specialists)
/// </summary>
/// <param name="SourceBagType">The type of the source bag.</param>
/// <param name="SourceSlot">The slot in the source bag.</param>
/// <param name="DestinationBagType">The type of the destination bag.</param>
/// <param name="DestinationSlot">The destination slot.</param>
[PacketHeader("mve", PacketSource.Client)]
[GenerateSerializer(true)]
public record MvePacket
(
    [PacketIndex(0)]
    BagType SourceBagType,
    [PacketIndex(1)]
    short SourceSlot,
    [PacketIndex(2)]
    BagType DestinationBagType,
    [PacketIndex(3)]
    short DestinationSlot
) : IPacket;