//
//  MateAttackStats.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Stats;

/// <summary>
/// Stats about mate attack.
/// </summary>
/// <param name="AttackUpgrade">The upgrade of attack.</param>
/// <param name="MinimumAttack">The minimum attack.</param>
/// <param name="MaximumAttack">The maximum attack.</param>
/// <param name="Precision">The precision or concentration.</param>
/// <param name="CriticalChance">The critical chance.</param>
/// <param name="CriticalRate">The critical rate.</param>
public record MateAttackStats
(
    short AttackUpgrade,
    int MinimumAttack,
    int MaximumAttack,
    int Precision,
    int CriticalChance,
    int CriticalRate
);