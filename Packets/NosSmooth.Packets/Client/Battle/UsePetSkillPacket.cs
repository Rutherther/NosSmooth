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
/// <param name="Unknown1">Unknown, 6 for Otter.</param>
/// <param name="Unknown2">Unknown, 9 for Otter.</param>
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
    [PacketIndex(4)]
    byte Unknown1,
    [PacketIndex(5)]
    byte Unknown2
) : IPacket;