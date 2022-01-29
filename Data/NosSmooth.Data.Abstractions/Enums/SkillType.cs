//
//  SkillType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Data.Abstractions.Enums;

/// <summary>
/// A type of a skill.
/// </summary>
public enum SkillType
{
    /// <summary>
    /// The skill is a passive, used automatically.
    /// </summary>
    Passive,

    /// <summary>
    /// The skill is for players.
    /// </summary>
    Player,

    /// <summary>
    /// UNKNOWN TODO.
    /// </summary>
    Upgrade,

    /// <summary>
    /// Unknown TODO.
    /// </summary>
    Emote,

    /// <summary>
    /// The skill is for monsters.
    /// </summary>
    Monster,

    /// <summary>
    /// The skill is for partners.
    /// </summary>
    Partner
}