//
//  Skill.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Infos;

namespace NosSmooth.Game.Data.Characters;

/// <summary>
/// Represents nostale skill entity.
/// </summary>
/// <param name="SkillVNum">The vnum of the skill.</param>
/// <param name="Level">The level of the skill. Unknown feature.</param>
/// <param name="Info">The information obtained from Data assembly.</param>
public record Skill(int SkillVNum, int? Level = default, ISkillInfo? Info = default)
{
    /// <summary>
    /// Gets the last time this skill was used.
    /// </summary>
    public DateTimeOffset LastUseTime { get; internal set; }

    /// <summary>
    /// Gets whether the skill is on cooldown.
    /// </summary>
    /// <remarks>
    /// This is set when the server sends sr packet,
    /// prefer to use this instead of checking the LastUseTime and Cooldown.
    /// </remarks>
    public bool IsOnCooldown { get; internal set; }
}