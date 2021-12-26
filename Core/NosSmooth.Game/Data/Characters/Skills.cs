//
//  Skills.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Characters;

/// <summary>
/// Holds skill of a Character.
/// </summary>
/// <param name="PrimarySkill">The VNum of the primary skill. This skill is used with the primary weapon. (Could be different for sp cards.)</param>
/// <param name="SecondarySkill">The VNum of the secondary skill. This skill is used with the secondary weapon. (Could be different for sp cards)</param>
/// <param name="OtherSkills">The VNums of other skills.</param>
public record Skills
(
    Skill PrimarySkill,
    Skill SecondarySkill,
    IReadOnlyList<Skill> OtherSkills
);