//
//  StPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using NosSmooth.Packets.Attributes;
using NosSmooth.Packets.Enums;

namespace NosSmooth.Packets.Packets.Server.Entities;

/// <summary>
/// Sent as a response to "ncif" packet.
/// </summary>
/// <param name="EntityType">The type of the entity.</param>
/// <param name="EntityId">The id of the entity.</param>
/// <param name="Level">The level of the entity.</param>
/// <param name="HeroLevel">The hero level. (for players only)</param>
/// <param name="HpPercentage">The current hp percentage.</param>
/// <param name="MpPercentage">The current mp percentage.</param>
/// <param name="Hp">The current amount of hp.</param>
/// <param name="Mp">The current amount of mp.</param>
[PacketHeader("st", PacketSource.Server)]
[GenerateSerializer]
public record StPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketIndex(2)]
    byte Level,
    [PacketIndex(3)]
    byte HeroLevel,
    [PacketIndex(4)]
    short HpPercentage,
    [PacketIndex(5)]
    short MpPercentage,
    [PacketIndex(6)]
    long Hp,
    [PacketIndex(7)]
    long Mp,
    [PacketListIndex(8, ListSeparator = ' ')]
    List<long> BuffVNums
) : IPacket;