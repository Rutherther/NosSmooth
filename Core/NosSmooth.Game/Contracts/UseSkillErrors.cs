//
//  UseSkillErrors.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Contracts;

/// <summary>
/// Errors for using a skill.
/// </summary>
public enum UseSkillErrors
{
    /// <summary>
    /// An unknown error has happened.
    /// </summary>
    Unknown,

    /// <summary>
    /// The character does not have enough ammo.
    /// </summary>
    NoAmmo,

    /// <summary>
    /// The character does not have enough mana.
    /// </summary>
    NoMana,

    /// <summary>
    /// There was no response from the server.
    /// </summary>
    NoResponse
}