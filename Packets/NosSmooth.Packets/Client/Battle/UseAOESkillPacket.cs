//
//  UseAOESkillPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client.Battle;

/// <summary>
/// Sent to use a skill that is targeted at position.
/// </summary>
/// <param name="SkillId">The id of the skill.</param>
/// <param name="PositionX">The x coordinate where to target to.</param>
/// <param name="PositionY">The y coordinate where to targe to.</param>
[PacketHeader("u_as", PacketSource.Client)]
[GenerateSerializer(true)]
public record UseAOESkillPacket
(
    [PacketIndex(0)]
    long SkillId,
    [PacketIndex(1)]
    short PositionX,
    [PacketIndex(2)]
    short PositionY
) : IPacket;