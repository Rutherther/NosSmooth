//
//  MovePacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Entities;

/// <summary>
/// The entity has moved to the given position.
/// </summary>
/// <param name="EntityType">The type of the entity that has moved.</param>
/// <param name="EntityId">The id of the entity.</param>
/// <param name="MapX">The x coordinate the entity has moved to.</param>
/// <param name="MapY">The y coordinate the entity has moved to.</param>
/// <param name="Speed">The speed of the entity.</param>
[PacketHeader("mv", PacketSource.Server)]
[GenerateSerializer(true)]
public record MovePacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketIndex(2)]
    short MapX,
    [PacketIndex(3)]
    short MapY,
    [PacketIndex(4)]
    byte Speed
) : IPacket;