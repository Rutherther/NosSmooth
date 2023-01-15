//
//  TpPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Maps;

/// <summary>
/// An entity teleport packet,
/// the entity jumps to the given position instantly.
/// </summary>
/// <param name="EntityType">The type of the entity.</param>
/// <param name="EntityId">The id of the entity.</param>
/// <param name="PositionX">The x coordinate the entity teleports to.</param>
/// <param name="PositionY">The y coordinate the entity teleports to.</param>
/// <param name="Unknown">Unknown TODO.</param>
[PacketHeader("tp", PacketSource.Server)]
[GenerateSerializer(true)]
public record TpPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketIndex(2)]
    short PositionX,
    [PacketIndex(3)]
    short PositionY,
    [PacketIndex(4)]
    sbyte Unknown
) : IPacket;