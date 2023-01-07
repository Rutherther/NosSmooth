//
//  MateArmorStats.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Stats;

/// <summary>
/// Stats about mate armor.
/// </summary>
/// <param name="DefenceUpgrade">The upgrade of defence.</param>
/// <param name="MeleeDefence">The melee defence.</param>
/// <param name="MeleeDefenceDodge">The melee dodge rate.</param>
/// <param name="RangedDefence">The ranged defence.</param>
/// <param name="RangedDodgeRate">The ranged dodge rate.</param>
/// <param name="MagicalDefence">The magical defence.</param>
public record MateArmorStats
(
    short DefenceUpgrade,
    int MeleeDefence,
    int MeleeDefenceDodge,
    int RangedDefence,
    int RangedDodgeRate,
    int MagicalDefence
);