//
//  RaidProgress.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Raids;

/// <summary>
/// A progress of a started <see cref="Raid"/>.
/// </summary>
/// <remarks>
/// Lockers are relevant before <see cref="RaidState.BossFight"/>,
/// when all lockers are unlocked, the boss room is opened.
/// </remarks>
/// <param name="MonsterLockerInitial">The number of monsters to kill.</param>
/// <param name="MonsterLockerCurrent">The number of monsters already killed.</param>
/// <param name="ButtonLockerInitial">The number of levers to pull.</param>
/// <param name="ButtonLockerCurrent">The number of levers already pulled.</param>
/// <param name="CurrentLives">The current number of lives.</param>
/// <param name="InitialLives">The maximum number of lives.</param>
public record RaidProgress
(
    short MonsterLockerInitial,
    short MonsterLockerCurrent,
    short ButtonLockerInitial,
    short ButtonLockerCurrent,
    short CurrentLives,
    short InitialLives
);