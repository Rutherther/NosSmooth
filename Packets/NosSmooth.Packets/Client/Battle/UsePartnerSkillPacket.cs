//
//  UsePartnerSkillPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client.Battle;

/// <summary>
/// Sent to use a pet skill.
/// </summary>
/// <param name="MateTransportId">The pet skill id.</param>
/// <param name="TargetEntityType">The target entity type.</param>
/// <param name="TargetId">The target id.</param>
/// <param name="SkillSlot">The slot of the skill.</param>
/// <param name="MapX">The x coordinate to target to, present if the skill is area skill.</param>
/// <param name="MapY">The y coordinate to target to, present if the skill is area skill.</param>
[PacketHeader("u_ps", PacketSource.Client)]
[GenerateSerializer(true)]
public record UsePartnerSkillPacket
(
    [PacketIndex(0)]
    long MateTransportId,
    [PacketIndex(1)]
    EntityType TargetEntityType,
    [PacketIndex(2)]
    long TargetId,
    [PacketIndex(3)]
    byte SkillSlot,
    [PacketIndex(4, IsOptional = true)]
    short? MapX,
    [PacketIndex(5, IsOptional = true)]
    short? MapY
) : IPacket;