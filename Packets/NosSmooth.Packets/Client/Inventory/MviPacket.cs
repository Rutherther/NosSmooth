//
//  MviPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Inventory;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client.Inventory;

/// <summary>
/// Move item in a bag from one position to another (swap items).
/// </summary>
/// <param name="BagType">The type of the bag to move items in.</param>
/// <param name="SourceSlot">The source slot.</param>
/// <param name="Amount">The amount to move.</param>
/// <param name="DestinationSlot">The destination slot.</param>
[PacketHeader("mvi", PacketSource.Client)]
[GenerateSerializer(true)]
public record MviPacket
(
    [PacketIndex(0)]
    BagType BagType,
    [PacketIndex(1)]
    short SourceSlot,
    [PacketIndex(2)]
    short Amount,
    [PacketIndex(3)]
    short DestinationSlot
) : IPacket;