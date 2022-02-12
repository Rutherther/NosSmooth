//
//  MlObjPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Miniland;

/// <summary>
/// Miniland object packet.
/// </summary>
/// <param name="Slot">The slot in the inventory.</param>
/// <param name="InUse">Whether the item is placed in the miniland.</param>
/// <param name="X">The x coordinate, if in use.</param>
/// <param name="Y">The y coordinate, if in use.</param>
/// <param name="Width">The width of the object.</param>
/// <param name="Height">The height of the object.</param>
/// <param name="Unknown">Unknown TODO.</param>
/// <param name="DurabilityPoints">The durability points of a minigame.</param>
/// <param name="Unknown1">Unknown TODO.</param>
/// <param name="Unknown2">Unknown TODO.</param>
[PacketHeader("mlobj", PacketSource.Server)]
[GenerateSerializer(true)]
public record MlObjPacket
(
    [PacketIndex(0)]
    short Slot,
    [PacketIndex(1)]
    bool InUse,
    [PacketIndex(2)]
    short X,
    [PacketIndex(3)]
    short Y,
    [PacketIndex(4)]
    byte Width,
    [PacketIndex(5)]
    byte Height,
    [PacketIndex(6)]
    byte Unknown,
    [PacketIndex(7)]
    int DurabilityPoints,
    [PacketIndex(8)]
    bool Unknown1,
    [PacketIndex(9)]
    bool Unknown2
) : IPacket;