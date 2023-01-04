//
//  ExtsPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Inventory;

/// <summary>
/// Inventory extensions (slots) of the player.
/// Each field contains the number of slots
/// in the given inventory bag.
/// </summary>
/// <param name="Type">Unknown TODO.</param>
/// <param name="EquipmentSlots">The number of slots in equipment bag.</param>
/// <param name="MainSlots">The number of slots in main bag.</param>
/// <param name="EtcSlots">The number of slots in etc bag.</param>
[PacketHeader("exts", PacketSource.Server)]
[GenerateSerializer(true)]
public record ExtsPacket
(
    [PacketIndex(0)]
    byte Type,
    [PacketIndex(1)]
    byte EquipmentSlots,
    [PacketIndex(2)]
    byte MainSlots,
    [PacketIndex(3)]
    byte EtcSlots
) : IPacket;