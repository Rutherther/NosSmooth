//
//  QSlotPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Quicklist;

/// <summary>
/// Quicklist slot information. (skills, items in quick list)
/// </summary>
/// <param name="Slot">The slot the information is about.</param>
/// <param name="DataSubPacket">The data (skills, items) in the slot.</param>
[PacketHeader("qslot", PacketSource.Server)]
[GenerateSerializer(true)]
public record QSlotPacket
(
    [PacketIndex(0)]
    byte Slot,
    [PacketListIndex(1, InnerSeparator = '.', ListSeparator = ' ')]
    IReadOnlyList<QSlotSubPacket> DataSubPacket
) : IPacket;