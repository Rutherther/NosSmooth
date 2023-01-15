//
//  CtPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Battle;

/// <summary>
/// A skill cast animation has begun.
/// <see cref="SuPacket"/> or <see cref="BsPacket"/> will be sent after
/// the skill is casted. (after cast time.)
/// </summary>
/// <param name="CasterEntityType">The type of the caster entity.</param>
/// <param name="CasterEntityId">The entity id of the caster.</param>
/// <param name="TargetEntityType">The type of the target entity.</param>
/// <param name="TargetEntityId">The entity id of the target.</param>
/// <param name="CastAnimation">The id of the cast animation.</param>
/// <param name="CastEffect">The id of teh cast effect.</param>
/// <param name="SkillVNum">The VNum of the used skill.</param>
[PacketHeader("ct", PacketSource.Server)]
[GenerateSerializer(true)]
public record CtPacket
(
    [PacketIndex(0)]
    EntityType CasterEntityType,
    [PacketIndex(1)]
    long CasterEntityId,
    [PacketIndex(2)]
    EntityType TargetEntityType,
    [PacketIndex(3)]
    long? TargetEntityId,
    [PacketIndex(4)]
    short? CastAnimation,
    [PacketIndex(5)]
    short? CastEffect,
    [PacketIndex(6)]
    int? SkillVNum
) : IPacket;