//
//  TargetType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Data.Abstractions.Enums;

/// <summary>
/// Type of a target of a skill.
/// </summary>
public enum TargetType
{
    /// <summary>
    /// The skill has a (enemy) target.
    /// </summary>
    Target,

    /// <summary>
    /// The skill can be targeted only on self.
    /// </summary>
    Self,

    /// <summary>
    /// The skill can be targeted on self or a (enemy) target.
    /// </summary>
    SelfOrTarget,

    /// <summary>
    /// The skill has no target. UNKNOWN TODO.
    /// </summary>
    NoTarget
}