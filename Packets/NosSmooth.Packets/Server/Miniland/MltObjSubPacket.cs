//
//  MltObjSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Miniland;

/// <summary>
/// Sub packet of <see cref="MltObjPacket"/>.
/// </summary>
/// <param name="VNum">The vnum of the item.</param>
/// <param name="Slot">The slot.</param>
/// <param name="X">The x coordinate.</param>
/// <param name="Y">The y coordinate.</param>
[PacketHeader("mltobjsub", PacketSource.Server)]
[GenerateSerializer(true)]
public record MltObjSubPacket
(
    [PacketIndex(0)]
    int VNum,
    [PacketIndex(1)]
    int Slot,
    [PacketIndex(2)]
    short X,
    [PacketIndex(3)]
    short Y
) : IPacket;