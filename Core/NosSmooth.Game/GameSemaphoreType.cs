//
//  GameSemaphoreType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game;

/// <summary>
/// Type of game semaphore.
/// </summary>
public enum GameSemaphoreType
{
    /// <summary>
    /// The semaphore for character.
    /// </summary>
    Character,

    /// <summary>
    /// The semaphore for inventory.
    /// </summary>
    Inventory,

    /// <summary>
    /// The semaphore for friends.
    /// </summary>
    Friends,

    /// <summary>
    /// The semaphore for family.
    /// </summary>
    Family,

    /// <summary>
    /// The semaphore for group.
    /// </summary>
    Group,

    /// <summary>
    /// The semaphore for skills.
    /// </summary>
    Skills,

    /// <summary>
    /// The semaphore for map.
    /// </summary>
    Map,

    /// <summary>
    /// The semaphore for raid.
    /// </summary>
    Raid,

    /// <summary>
    /// The semaphore for mates.
    /// </summary>
    Mates
}