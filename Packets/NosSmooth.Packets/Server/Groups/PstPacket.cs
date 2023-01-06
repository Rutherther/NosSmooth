//
//  PstPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Entities;
using NosSmooth.Packets.Enums.Mates;
using NosSmooth.Packets.Enums.Players;
using NosSmooth.Packets.Server.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Groups;

/// <summary>
/// Party status packet.
/// </summary>
/// <param name="EntityType">The type of the entity.</param>
/// <param name="GroupPosition">The position in the group of the entity</param>
/// <param name="HpPercentage">The hp percentage of the entity.</param>
/// <param name="MpPercentage">The mp percentage of the entity.</param>
/// <param name="Hp">The hp of the entity.</param>
/// <param name="Mp">The mp of the entity.</param>
/// <param name="PlayerClass">The player class, present for player.</param>
/// <param name="PlayerSex">The sex of the player, present for player.</param>
/// <param name="PlayerMorphVNum">The player morph vnum, present for player.</param>
/// <param name="Effects">The effects vnums.</param>
[PacketHeader("pst", PacketSource.Server)]
[GenerateSerializer(true)]
public record PstPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketConditionalIndex(2, "EntityType", false, EntityType.Player)]
    byte? GroupPosition,
    [PacketConditionalIndex(3, "EntityType", false, EntityType.Npc)]
    MateType? MateType,
    [PacketIndex(4)]
    byte HpPercentage,
    [PacketIndex(5)]
    byte MpPercentage,
    [PacketIndex(6)]
    int Hp,
    [PacketIndex(7)]
    int Mp,
    [PacketIndex(8)]
    PlayerClass? PlayerClass,
    [PacketIndex(9)]
    SexType? PlayerSex,
    [PacketIndex(10)]
    long? PlayerMorphVNum,
    [PacketIndex(11, IsOptional = true)]
    IReadOnlyList<EffectsSubPacket>? Effects
) : IPacket;