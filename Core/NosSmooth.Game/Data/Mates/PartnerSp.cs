//
//  PartnerSp.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Characters;

namespace NosSmooth.Game.Data.Mates;

/// <summary>
/// An sp of the partner.
/// </summary>
/// <param name="VNum">The vnum of the sp item.</param>
/// <param name="AgilityPercentage">The agility percentage for acquiring skills.</param>
/// <param name="Skill1">Information about the first skill of the partner.</param>
/// <param name="Skill2">Information about the second skill of the partner.</param>
/// <param name="Skill3">Information about the third skill of the partner.</param>
public record PartnerSp
(
    long VNum,
    byte? AgilityPercentage,
    PartnerSkill? Skill1,
    PartnerSkill? Skill2,
    PartnerSkill? Skill3
);