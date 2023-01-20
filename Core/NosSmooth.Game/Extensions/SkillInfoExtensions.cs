//
//  SkillInfoExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Infos;

namespace NosSmooth.Game.Extensions;

/// <summary>
/// Extension methods for <see cref="ISkillInfo"/>.
/// </summary>
public static class SkillInfoExtensions
{
    /// <summary>
    /// Check whether the given skill is a combo skill.
    /// </summary>
    /// <param name="skillInfo">The info about the skill.</param>
    /// <param name="morph">The morph to validate matches the morph of the skill.</param>
    /// <returns>Whether the skill is a combo skill.</returns>
    public static bool IsComboSkill(this ISkillInfo skillInfo, int? morph = default)
        => skillInfo.SpecialCost == 999 && skillInfo.CastId > 10 && (morph is null || skillInfo.MorphOrUpgrade == morph.Value);
}