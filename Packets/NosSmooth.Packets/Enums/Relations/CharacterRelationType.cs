//
//  CharacterRelationType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Packets.Enums.Relations;

/// <summary>
/// Type of character-player relation.
/// </summary>
public enum CharacterRelationType
{
    /// <summary>
    /// The player is blocked.
    /// </summary>
    Blocked = -1,

    /// <summary>
    /// The player is a friend.
    /// </summary>
    Friend = 0,

    /// <summary>
    /// The player is a hidden spouse.
    /// </summary>
    HiddenSpouse = 2,

    /// <summary>
    /// The player is spouse.
    /// </summary>
    Spouse = 5,
}