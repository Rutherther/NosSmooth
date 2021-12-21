//
//  GameOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game;

/// <summary>
/// Options for <see cref="Game"/>.
/// </summary>
public class GameOptions
{
    /// <summary>
    /// Duration to cache entities for after changing maps in seconds.
    /// </summary>
    public ulong EntityCacheDuration { get; set; }
}