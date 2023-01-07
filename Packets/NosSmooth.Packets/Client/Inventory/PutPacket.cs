//
//  PutPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Inventory;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client.Inventory;

/// <summary>
/// Drop an item from inventory.
/// </summary>
[PacketHeader("put", PacketSource.Client)]
[GenerateSerializer(true)]
public record PutPacket
(
    [PacketIndex(0)]
    BagType BagType,
    [PacketIndex(1)]
    short Slot,
    [PacketIndex(2)]
    short Amount
) : IPacket;