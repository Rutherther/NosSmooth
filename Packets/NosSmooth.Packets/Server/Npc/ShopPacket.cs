//
//  ShopPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.Npc;

/// <summary>
/// A shop on the map.
/// </summary>
/// <param name="EntityType">The type of the entity.</param>
/// <param name="EntityVNum">The entity vnum.</param>
/// <param name="ShopId">The id of the shop.</param>
/// <param name="MenuType">The menu type, unknown values.</param>
/// <param name="ShopType">The shop type, unkonwn values.</param>
/// <param name="Name">The name of the shop. (shown above the NPC)</param>
[PacketHeader("shop", PacketSource.Server)]
[GenerateSerializer(true)]
public record ShopPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityVNum,
    [PacketIndex(2)]
    long ShopId,
    [PacketIndex(3)]
    byte MenuType,
    [PacketIndex(4, IsOptional = true)]
    byte? ShopType,
    [PacketGreedyIndex(5, IsOptional = true)]
    NameString? Name
) : IPacket;