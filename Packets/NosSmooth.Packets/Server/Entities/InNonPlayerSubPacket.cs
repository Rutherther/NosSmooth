//
//  InNonPlayerSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.Entities;

/// <summary>
/// Sub packet of <see cref="InPacket"/> present if the in packet
/// is for a monster or npc.
/// </summary>
/// <param name="HpPercentage">The hp percentage of the entity.</param>
/// <param name="MpPercentage">The mp percentage of the entity.</param>
/// <param name="Dialog">Unknown TODO</param>
/// <param name="Faction">The faction of the entity.</param>
/// <param name="GroupEffect">Unknown TODO</param>
/// <param name="OwnerId">The id of the owner entity.</param>
/// <param name="SpawnEffect">The effect the entity does on spawning.</param>
/// <param name="IsSitting">Whether the entity is sitting.</param>
/// <param name="MorphVNum">The id of the morph (for special cards an such).</param>
/// <param name="Name">The name of the entity, if any.</param>
/// <param name="Unknown">Unknown.</param>
/// <param name="Unknown2">Unknown.</param>
/// <param name="Unknown3">Unknown.</param>
/// <param name="Skill1">The first skill VNum of the entity.</param>
/// <param name="Skill2">The second skill VNum of the entity.</param>
/// <param name="Skill3">The third skill VNum of the entity.</param>
/// <param name="SkillRank1">The rank of the first skill.</param>
/// <param name="SkillRank2">The rank of the second skill.</param>
/// <param name="SkillRank3">The rank of the third skill.</param>
/// <param name="IsInvisible">Whether the entity is invisible.</param>
/// <param name="Unknown4">Unknown.</param>
/// <param name="Unknown5">Unknown.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record InNonPlayerSubPacket
(
    [PacketIndex(0)]
    byte HpPercentage,
    [PacketIndex(1)]
    byte MpPercentage,
    [PacketIndex(2)]
    short Dialog,
    [PacketIndex(3)]
    FactionType Faction,
    [PacketIndex(4)]
    short GroupEffect,
    [PacketIndex(5)]
    long? OwnerId,
    [PacketIndex(6)]
    SpawnEffect SpawnEffect,
    [PacketIndex(7)]
    bool IsSitting,
    [PacketIndex(8)]
    long? MorphVNum,
    [PacketIndex(9)]
    NameString? Name,
    [PacketIndex(10)]
    string? Unknown,
    [PacketIndex(11)]
    string? Unknown2,
    [PacketIndex(12)]
    string? Unknown3,
    [PacketIndex(13)]
    short Skill1,
    [PacketIndex(14)]
    short Skill2,
    [PacketIndex(15)]
    short Skill3,
    [PacketIndex(16)]
    short SkillRank1,
    [PacketIndex(17)]
    short SkillRank2,
    [PacketIndex(18)]
    short SkillRank3,
    [PacketIndex(19)]
    bool IsInvisible,
    [PacketIndex(20)]
    string? Unknown4,
    [PacketIndex(21)]
    string? Unknown5
) : IPacket;