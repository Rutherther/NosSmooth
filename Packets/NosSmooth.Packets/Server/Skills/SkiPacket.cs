//
//  SkiPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Skills;

/// <summary>
/// Information about skills, sent when changing sp, map etc.
/// </summary>
/// <param name="Unknown">Unknown TODO.</param>
/// <param name="PrimarySkillVNum">The primary skill vnum.</param>
/// <param name="SecondarySkillVNum">The secondary skill vnum. (may be same as primary in some cases, like for special cards).</param>
/// <param name="SkillSubPackets">The rest of the skills.</param>
[PacketHeader("ski", PacketSource.Server)]
[GenerateSerializer(true)]
public record SkiPacket
(
    [PacketIndex(0)]
    int? Unknown,
    [PacketIndex(1)]
    int PrimarySkillVNum,
    [PacketIndex(2)]
    int SecondarySkillVNum,
    [PacketListIndex(3, InnerSeparator = '|', ListSeparator = ' ')]
    IReadOnlyList<SkiSubPacket> SkillSubPackets
) : IPacket;