﻿//
//  SkiPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Skills;

/// <summary>
/// Information about skills, sent when changing sp, map etc.
/// </summary>
/// <param name="PrimarySkillId">The primary skill id.</param>
/// <param name="SecondarySkillId">The secondary skill id. (may be same as primary in some cases, like for special cards).</param>
/// <param name="SkillSubPackets">The rest of the skills.</param>
[PacketHeader("ski", PacketSource.Server)]
[GenerateSerializer(true)]
public record SkiPacket
(
    [PacketIndex(0)]
    long PrimarySkillId,
    [PacketIndex(1)]
    long SecondarySkillId,
    [PacketListIndex(2, InnerSeparator = '|', ListSeparator = ' ')]
    IReadOnlyList<SkiSubPacket> SkillSubPackets
) : IPacket;