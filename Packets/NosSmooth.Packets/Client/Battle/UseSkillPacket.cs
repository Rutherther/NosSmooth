//
//  UseSkillPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client.Battle;

/// <summary>
/// Sent to use regular skill.
/// </summary>
/// <remarks>
/// Contains the position etc. for dashes.
/// </remarks>
[PacketHeader("u_s", PacketSource.Client)]
[GenerateSerializer(true)]
public record UseSkillPacket
(
    [PacketIndex(0)]
    long SkillId,
    [PacketIndex(1)]
    EntityType TargetEntityType,
    [PacketIndex(2)]
    long TargetId,
    [PacketIndex(3, IsOptional = true)]
    short? MapX,
    [PacketIndex(4, IsOptional = true)]
    short? MapY
) : IPacket;