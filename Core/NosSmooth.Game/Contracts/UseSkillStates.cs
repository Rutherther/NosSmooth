//
//  UseSkillStates.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Contracts;

/// <summary>
/// States for contract for using a skill.
/// </summary>
public enum UseSkillStates
{
    /// <summary>
    /// Skill use was not executed yet.
    /// </summary>
    None,

    /// <summary>
    /// A skill use packet (u_s, u_as, etc.) was used,
    /// awaiting a response from the server.
    /// </summary>
    SkillUseRequested,

    /// <summary>
    /// The server has responded with a skill use,
    /// (after cast time), the information about
    /// the caster, target is filled.
    /// </summary>
    SkillUsedResponse,

    /// <summary>
    /// Fired 1000 ms after <see cref="SkillUsedResponse"/>,
    /// the character may move after this.
    /// </summary>
    CharacterRestored
}