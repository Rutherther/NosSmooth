//
//  CondPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Entities;

/// <summary>
/// Cond packet is sent when the player is moving or when attacking an entity.
/// </summary>
/// <param name="EntityType">The type of the entity.</param>
/// <param name="EntityId">The id of the entity.</param>
/// <param name="CantAttack">Whether the entity cant attack.</param>
/// <param name="CantMove">Whether the entity cant move.</param>
/// <param name="Speed">The speed of the entity.</param>
[PacketHeader("cond", PacketSource.Server)]
[GenerateSerializer(true)]
public record CondPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketIndex(2)]
    bool CantAttack,
    [PacketIndex(3)]
    bool CantMove,
    [PacketIndex(4)]
    int Speed
) : IPacket;