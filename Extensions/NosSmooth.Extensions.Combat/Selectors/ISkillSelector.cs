//
//  ISkillSelector.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Characters;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Selectors;

/// <summary>
/// Selects a skill to use from a possible skills.
/// </summary>
public interface ISkillSelector
{
    /// <summary>
    /// Gets the skill to use.
    /// </summary>
    /// <param name="usableSkills">The skills that may be used. Won't contain skills the user doesn't have mana for and that are on cooldown.</param>
    /// <returns>The skill to use, or an error.</returns>
    public Result<Skill> GetSelectedSkill(IEnumerable<Skill> usableSkills);
}