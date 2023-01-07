//
//  Resistance.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Stats;

/// <summary>
/// Stats about resistance of character or mate.
/// </summary>
/// <param name="FireResistance">The fire resistance percentage.</param>
/// <param name="WaterResistance">The water resistance percentage.</param>
/// <param name="LightResistance">The light resistance percentage.</param>
/// <param name="DarkResistance">The dark resistance percentage.</param>
public record Resistance
(
    short FireResistance,
    short WaterResistance,
    short LightResistance,
    short DarkResistance
);