//
//  MateArmorStats.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Stats;

public record MateArmorStats
(
    short DefenceUpgrade,
    int MeleeDefence,
    int MeleeDefenceDodge,
    int RangeDefence,
    int RangeDodgeRate,
    int MagicalDefence
);