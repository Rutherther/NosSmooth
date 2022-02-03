//
//  BsPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Battle;

/// <summary>
/// An AoE skill used packet.
/// </summary>
/// <param name="CasterEntityType">The caster entity type.</param>
/// <param name="CasterEntityId">The caster entity id.</param>
/// <param name="X">The x coordinate of the skill.</param>
/// <param name="Y"></param>
/// <param name="Cooldown"></param>
/// <param name="AttackAnimationId"></param>
/// <param name="EffectId"></param>
/// <param name="Unknown1"></param>
/// <param name="Unknown2"></param>
/// <param name="Unknown3"></param>
/// <param name="Unknown4"></param>
/// <param name="Unknown5"></param>
/// <param name="Unknown6"></param>
/// <param name="Unknown7"></param>
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
    long EffectId,
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