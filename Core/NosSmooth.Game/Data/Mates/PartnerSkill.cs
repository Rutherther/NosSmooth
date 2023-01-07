//
//  PartnerSkill.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Packets.Enums.Mates;

namespace NosSmooth.Game.Data.Mates;

public record PartnerSkill
(
    int SkillVNum,
    PartnerSkillRank? Rank,
    ISkillInfo? Info
) : Skill(SkillVNum, null, Info);