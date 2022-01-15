//
//  SkiSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Skills;

/// <summary>
/// Sub packet for <see cref="SkiPacket"/> containing information about a skill.
/// </summary>
/// <param name="SkillId">The id of the skill.</param>
/// <param name="Rank">The rank of the skill.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record SkiSubPacket
(
    [PacketIndex(0)]
    long SkillId,
    [PacketIndex(1)]
    byte Rank
) : IPacket;