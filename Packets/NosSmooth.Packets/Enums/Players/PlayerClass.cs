//
//  PlayerClass.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Packets.Enums.Players;

/// <summary>
/// The class of the player.
/// </summary>
public enum PlayerClass
{
    /// <summary>
    /// Adventurer class.
    /// </summary>
    /// <remarks>
    /// After creating a character, this is the first class.
    /// The player can change the class at level 15, job level 20.
    /// </remarks>
    Adventurer = 0,

    /// <summary>
    /// Swordsman class.
    /// </summary>
    /// <remarks>
    /// Primary weapon is a sword, secondary weapon is a crossbow.
    /// </remarks>
    Swordsman = 1,

    /// <summary>
    /// Archer class.
    /// </summary>
    /// <remarks>
    /// Primary weapon is a bow,
    /// secondary weapon is a dagger.
    /// </remarks>
    Archer = 2,

    /// <summary>
    /// Mage class.
    /// </summary>
    /// <remarks>
    /// Primary weapon is a wand,
    /// secondary weapon is magical gun.
    /// </remarks>
    Mage = 3,

    /// <summary>
    /// Martial artist class.
    /// </summary>
    /// <remarks>
    /// Can be created after reaching level 80 on any of the characters.
    /// Then character with that class can be created on the same account.
    /// </remarks>
    MartialArtist = 4
}