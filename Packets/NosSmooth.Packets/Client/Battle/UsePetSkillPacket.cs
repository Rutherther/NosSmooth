//
//  UsePetSkillPacket.cs
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
/// <param name="Unknown">Unknown, seems to always be 1.</param>
/// <param name="MapX">The x coordinate of the pet.</param>
/// <param name="MapY">The y coordinate of the pet.</param>
[PacketHeader("u_pet", PacketSource.Client)]
[GenerateSerializer(true)]
public record UsePetSkillPacket
(
    [PacketIndex(0)]
    long MateTransportId,
    [PacketIndex(1)]
    EntityType TargetEntityType,
    [PacketIndex(2)]
    long TargetId,
    [PacketIndex(3)]
    byte Unknown,
    [PacketIndex(4, IsOptional = true)]
    short? MapX,
    [PacketIndex(5, IsOptional = true)]
    short? MapY
) : IPacket;