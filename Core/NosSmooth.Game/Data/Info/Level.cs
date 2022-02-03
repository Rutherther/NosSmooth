//
//  Level.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Info;

/// <summary>
/// Represents a level, such as job level, hero level, character level.
/// </summary>
/// <param name="Lvl">The level.</param>
/// <param name="Xp">Current xp.</param>
/// <param name="XpLoad">Maximum xp of the current level.</param>
public record Level(short Lvl, long Xp, long XpLoad);