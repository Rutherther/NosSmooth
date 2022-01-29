//
//  HitType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Data.Abstractions.Enums;

/// <summary>
/// A hit type of a skill.
/// </summary>
public enum HitType
{
    /// <summary>
    /// The skill is for just one target.
    /// </summary>
    TargetOnly,

    /// <summary>
    /// The skill will hit enemies in a zone.
    /// </summary>
    /// <remarks>
    /// Can be AOE skill or a targeted skill that targets more enemies.
    /// </remarks>
    EnemiesInZone,

    /// <summary>
    /// The skill will hit allies in a zone, this is a buff.
    /// </summary>
    AlliesInZone,

    /// <summary>
    /// UNKNOWN TODO.
    /// </summary>
    SpecialArea
}