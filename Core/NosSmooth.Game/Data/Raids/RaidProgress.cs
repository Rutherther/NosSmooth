//
//  RaidProgress.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Raids;

public record RaidProgress
(
    short MonsterLockerInitial,
    short MonsterLockerCurrent,
    short ButtonLockerInitial,
    short ButtonLockerCurrent,
    short CurrentLives,
    short InitialLives
);