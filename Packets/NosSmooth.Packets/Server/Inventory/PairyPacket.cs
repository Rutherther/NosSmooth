//
//  PairyPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Inventory;

/// <summary>
/// Information about a fairy of an entity.
/// </summary>
/// <param name="EntityType">The entity type.</param>
/// <param name="EntityId">The id of the entity.</param>
/// <param name="MoveType">The fairy's move type.</param>
/// <param name="Element">The fairy's element.</param>
/// <param name="ElementRate">The fairy's element rate.</param>
/// <param name="MorphVNum">The fairy's morph vnum.</param>
[PacketHeader("pairy", PacketSource.Server)]
[GenerateSerializer(true)]
public record PairyPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketIndex(2)]
    int MoveType,
    [PacketIndex(3)]
    Element Element,
    [PacketIndex(4)]
    int ElementRate,
    [PacketIndex(5)]
    int MorphVNum
) : IPacket;