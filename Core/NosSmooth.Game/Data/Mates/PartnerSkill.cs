//
//  PartnerSkill.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Packets.Enums.Mates;

namespace NosSmooth.Game.Data.Mates;

/// <summary>
/// A skill of a partner's sp.
/// </summary>
/// <param name="SkillVNum">The vnum of the skill.</param>
/// <param name="Rank">The partner rank of the skill.</param>
/// <param name="Info">The info of the skill.</param>
public record PartnerSkill
(
    int SkillVNum,
    PartnerSkillRank? Rank,
    ISkillInfo? Info
) : Skill(SkillVNum, null, Info);