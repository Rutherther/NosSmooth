//
//  QSlotSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Packets;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Quicklist;

/// <summary>
/// A sub packet of <see cref="QSlotPacket"/>
/// containing information about an operation for a box in the slot.
/// </summary>
/// <param name="Type">The type of the operation.</param>
/// <param name="Slot">Unknown TODO.</param>
/// <param name="Position">Unknown TODO.</param>
/// <param name="Data">Unknown TODO.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record QSlotSubPacket
(
    [PacketIndex(0)]
    QSlotType Type,
    [PacketIndex(1)]
    short? Slot,
    [PacketIndex(2)]
    short? Position,
    [PacketIndex(3, IsOptional = true)]
    short? Data
) : IPacket;