//
//  MateAttackStats.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Stats;

public record MateAttackStats
(
    short AttackUpgrade,
    int MinimumAttack,
    int MaximumAttack,
    int Precision,
    int CriticalChance,
    int CriticalRate
);