//
//  UseSkillPolicy.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Extensions.Combat.Errors;
using NosSmooth.Extensions.Combat.Selectors;
using NosSmooth.Game.Data.Characters;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Policies;

/// <summary>
/// The policy to use a skill.
/// </summary>
/// <param name="PreferTargetedSkills">Whether to prefer targeted skills (true) or area skills (false).</param>
/// <param name="AllowedSkillVNums">The vnums of the skills that are allowed to be used.</param>
public record UseSkillPolicy(bool PreferTargetedSkills, int[]? AllowedSkillVNums)
    : ISkillSelector
{
    /// <inheritdoc />
    public Result<Skill> GetSelectedSkill(IEnumerable<Skill> usableSkills)
    {
        var skills = usableSkills.Where(x => CanBeUsed(x))
            .Reverse();

        if (PreferTargetedSkills)
        {
            skills = skills.OrderBy(x => x.Info!.HitType == HitType.EnemiesInZone ? 1 : 0);
        }

        var skill = skills.FirstOrDefault();
        if (skill is null)
        {
            return new SkillNotFoundError();
        }

        return skill;
    }

    private bool CanBeUsed(Skill skill)
    {
        if (AllowedSkillVNums is not null && !AllowedSkillVNums.Contains(skill.SkillVNum))
        {
            return false;
        }

        if (skill.Info is null)
        {
            return false;
        }

        return skill.Info.HitType is HitType.EnemiesInZone or HitType.TargetOnly
            && skill.Info.TargetType is TargetType.Target or TargetType.NoTarget;
    }
}