//
//  AtPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Maps;

/// <summary>
/// At packet. Sent when unknown TODO.
/// </summary>
/// <param name="CharacterId">The id of the character.</param>
/// <param name="MapId">The map id.</param>
/// <param name="X">The x coordinate of the character.</param>
/// <param name="Y">The y coordinate of the character.</param>
[PacketHeader("at", PacketSource.Server)]
[GenerateSerializer(true)]
public record AtPacket
(
    [PacketIndex(0)]
    long CharacterId,
    [PacketIndex(1)]
    int MapId,
    [PacketIndex(2)]
    short X,
    [PacketIndex(3)]
    short Y
) : IPacket;