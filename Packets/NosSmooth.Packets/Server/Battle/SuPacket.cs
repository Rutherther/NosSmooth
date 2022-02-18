//
//  SuPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Battle;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Battle;

/// <summary>
/// Represents skill used.
/// </summary>
/// <param name="CasterEntityType">The type of the caster entity.</param>
/// <param name="CasterEntityId">The entity id of the caster.</param>
/// <param name="TargetEntityType">The type of the target entity.</param>
/// <param name="TargetEntityId">The entity id of the target.</param>
/// <param name="SkillVNum">The VNum of the used skill.</param>
/// <param name="SkillCooldown">The cooldown of the skill.</param>
/// <param name="AttackAnimation">The number of the attack animation.</param>
/// <param name="SkillEffect">The number of the skill effect.</param>
/// <param name="PositionX">The x position where the skill target is.</param>
/// <param name="PositionY">The y position where the skill target is.</param>
/// <param name="TargetIsAlive">Whether the target of the skill is alive.</param>
/// <param name="HpPercentage">The hp percentage of the entity after the attack.</param>
/// <param name="Damage">The damage the entity has taken.</param>
/// <param name="HitMode">The hit mode.</param>
/// <param name="SkillTypeMinusOne">The skill type of the skill.</param>
[PacketHeader("su", PacketSource.Server)]
[GenerateSerializer(true)]
public record SuPacket
(
    [PacketIndex(0)]
    EntityType CasterEntityType,
    [PacketIndex(1)]
    long CasterEntityId,
    [PacketIndex(2)]
    EntityType TargetEntityType,
    [PacketIndex(3)]
    long TargetEntityId,
    [PacketIndex(4)]
    int SkillVNum,
    [PacketIndex(5)]
    long SkillCooldown,
    [PacketIndex(6)]
    long AttackAnimation,
    [PacketIndex(7)]
    long SkillEffect,
    [PacketIndex(8)]
    short PositionX,
    [PacketIndex(9)]
    short PositionY,
    [PacketIndex(10)]
    bool TargetIsAlive,
    [PacketIndex(11)]
    byte HpPercentage,
    [PacketIndex(12)]
    uint Damage,
    [PacketIndex(13)]
    HitMode? HitMode,
    [PacketIndex(14)]
    int SkillTypeMinusOne
) : IPacket;
