//
//  InPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Attributes;
using NosSmooth.Packets.Common;
using NosSmooth.Packets.Enums;

namespace NosSmooth.Packets.Packets.Server.Entities;

/// <summary>
/// There is a new entity on the map present.
/// </summary>
/// <param name="EntityType">The type of the entity.</param>
/// <param name="Name">The name of the entity, present only for players.</param>
/// <param name="VNum">The vnum of the entity, present only for non players.</param>
/// <param name="Unknown">Unknown value present only for players. It's always "-".</param>
/// <param name="EntityId">The id of the entity.</param>
/// <param name="PositionX">The x coordinate the entity is at.</param>
/// <param name="PositionY">The y coordinate the entity is at.</param>
/// <param name="Direction">The direction the entity is looking, present only for non-objects.</param>
/// <param name="PlayerSubPacket">The player data sub packet present only for players.</param>
/// <param name="ItemSubPacket">The item data sub packet present only for objects.</param>
/// <param name="NonPlayerSubPacket">The non player data sub packet present only for npcs and monsters.</param>
[PacketHeader("in", PacketSource.Server)]
[GenerateSerializer]
public record InPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketConditionalIndex(1, "EntityType", false,  EntityType.Player)]
    NameString? Name,
    [PacketConditionalIndex(2, "EntityType", true, EntityType.Player)]
    long? VNum,
    [PacketConditionalIndex(3, "EntityType", false, EntityType.Player)]
    string? Unknown,
    [PacketIndex(4)]
    long EntityId,
    [PacketIndex(5)]
    short PositionX,
    [PacketIndex(6)]
    short PositionY,
    [PacketConditionalIndex(7, "EntityType", true, EntityType.Object)]
    byte? Direction,
    [PacketConditionalIndex(8, "EntityType", false, EntityType.Player, InnerSeparator = ' ')]
    InPlayerSubPacket? PlayerSubPacket,
    [PacketConditionalIndex(9, "EntityType", false, EntityType.Object, InnerSeparator = ' ')]
    InItemSubPacket? ItemSubPacket,
    [PacketConditionalIndex(10, "EntityType", true, EntityType.Player, EntityType.Object, InnerSeparator = ' ')]
    InNonPlayerSubPacket? NonPlayerSubPacket
) : IPacket;