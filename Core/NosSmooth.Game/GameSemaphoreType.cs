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
    /// The semaphore for the character.
    /// </summary>
    Character,

    /// <summary>
    /// The semaphore for the map.
    /// </summary>
    Map,

    /// <summary>
    /// The semaphore for the raid.
    /// </summary>
    Raid
}