//
//  ISkillInfo.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Data.Abstractions.Language;

namespace NosSmooth.Data.Abstractions.Infos;

/// <summary>
/// The NosTale skill information.
/// </summary>
public interface ISkillInfo : IVNumInfo
{
    /// <summary>
    /// Gets the translatable name of the skill.
    /// </summary>
    TranslatableString Name { get; }

    /// <summary>
    /// Gets the tile range of the skill.
    /// </summary>
    short Range { get; }

    /// <summary>
    /// Gets the zone tiles range.
    /// </summary>
    short ZoneRange { get; }

    /// <summary>
    /// Gets the time it takes to cast this skill. Units UNKNOWN TODO.
    /// </summary>
    int CastTime { get; }

    /// <summary>
    /// Gets the time of the cooldown. Units UNKNOWN TODO.
    /// </summary>
    int Cooldown { get; }

    /// <summary>
    /// Gets the type of the skill.
    /// </summary>
    SkillType SkillType { get; }

    /// <summary>
    /// Gets the mana points the skill cast costs.
    /// </summary>
    int MpCost { get; }

    /// <summary>
    /// Gets the cast id of the skill used in u_s, su packets.
    /// </summary>
    short CastId { get; }

    /// <summary>
    /// Gets the type of the target.
    /// </summary>
    TargetType TargetType { get; }

    /// <summary>
    /// Gets the hit type of the skill.
    /// </summary>
    HitType HitType { get; }
}