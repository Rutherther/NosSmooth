//
//  SkillsExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Characters;
using Remora.Results;

namespace NosSmooth.Game.Extensions;

/// <summary>
/// Contains extension methods for <see cref="Skills"/>.
/// </summary>
public static class SkillsExtensions
{
    /// <summary>
    /// Tries to get the skill of the specified vnum.
    /// </summary>
    /// <param name="skills">The skills of the player.</param>
    /// <param name="castId">The cast id to search for.</param>
    /// <returns>The skill, if found.</returns>
    public static Result<Skill> TryGetSkillByCastId(this Skills skills, short castId)
    {
        if (skills.PrimarySkill.Info?.CastId == castId)
        {
            return skills.PrimarySkill;
        }

        if (skills.SecondarySkill.Info?.CastId == castId)
        {
            return skills.SecondarySkill;
        }

        foreach (Skill skill in skills.OtherSkills)
        {
            if (skill.Info?.CastId == castId)
            {
                return skill;
            }
        }

        return new NotFoundError();
    }

    /// <summary>
    /// Tries to get the skill of the specified vnum.
    /// </summary>
    /// <param name="skills">The skills of the player.</param>
    /// <param name="skillVNum">The vnum to search for.</param>
    /// <returns>The skill, if found.</returns>
    public static Result<Skill> TryGetSkillByVNum(this Skills skills, long skillVNum)
    {
        if (skills.PrimarySkill.SkillVNum == skillVNum)
        {
            return skills.PrimarySkill;
        }

        if (skills.SecondarySkill.SkillVNum == skillVNum)
        {
            return skills.SecondarySkill;
        }

        foreach (Skill skill in skills.OtherSkills)
        {
            if (skill.SkillVNum == skillVNum)
            {
                return skill;
            }
        }

        return new NotFoundError();
    }
}