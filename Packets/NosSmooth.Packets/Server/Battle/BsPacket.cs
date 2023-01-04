//
//  BsPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Battle;

/// <summary>
/// An AoE skill used packet.
/// </summary>
/// <param name="CasterEntityType">The caster entity type.</param>
/// <param name="CasterEntityId">The caster entity id.</param>
/// <param name="X">The x coordinate where to use the skill.</param>
/// <param name="Y">The y coordinate where to use the skill.</param>
/// <param name="SkillVNum">The vnum of the AoE skill used.</param>
/// <param name="Cooldown">The cooldown of the skill. (seconds time 10)</param>
/// <param name="AttackAnimationId">The attack animation id.</param>
/// <param name="EffectId">The effect id.</param>
/// <param name="Unknown1">Unknown TODO.</param>
/// <param name="Unknown2">Unknown TODO.</param>
/// <param name="Unknown3">Unknown TODO.</param>
/// <param name="Unknown4">Unknown TODO.</param>
/// <param name="Unknown5">Unknown TODO.</param>
/// <param name="Unknown6">Unknown TODO.</param>
/// <param name="Unknown7">Unknown TODO.</param>
[PacketHeader("bs", PacketSource.Server)]
[GenerateSerializer(true)]
public record BsPacket
(
    [PacketIndex(0)]
    EntityType CasterEntityType,
    [PacketIndex(1)]
    long CasterEntityId,
    [PacketIndex(2)]
    short X,
    [PacketIndex(3)]
    short Y,
    [PacketIndex(4)]
    int SkillVNum,
    [PacketIndex(5)]
    short Cooldown,
    [PacketIndex(6)]
    long AttackAnimationId,
    [PacketIndex(7)]
    long? EffectId,
    [PacketIndex(8)]
    int Unknown1,
    [PacketIndex(9)]
    int Unknown2,
    [PacketIndex(10)]
    int Unknown3,
    [PacketIndex(11)]
    int Unknown4,
    [PacketIndex(12)]
    int Unknown5,
    [PacketIndex(13)]
    int Unknown6,
    [PacketIndex(14)]
    int Unknown7
) : IPacket;