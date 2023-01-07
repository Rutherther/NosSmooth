//
//  ScNSkillSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Mates;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Mates;

/// <summary>
/// A sub packet of <see cref="ScNPacket"/>
/// containing information about partner's
/// sp skill.
/// </summary>
/// <param name="SkillVNum">The vnum of the skill.</param>
/// <param name="Rank">The rank of the skill.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record ScNSkillSubPacket
(
    [PacketIndex(0)]
    int? SkillVNum,
    [PacketIndex(1, IsOptional = true)]
    PartnerSkillRank? Rank
) : IPacket;