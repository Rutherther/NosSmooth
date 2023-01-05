//
//  ItemType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
#pragma warning disable CS1591

namespace NosSmooth.Data.Abstractions.Enums;

/// <summary>
/// Type of an item.
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Self-explanatory.")]
public enum ItemType
{
    None = -1,
    Weapon = 0,
    Armor = 1,
    Fashion = 2,
    Jewelery = 3,
    Specialist = 4,
    Box = 5,
    Shell = 6,
    Main = 10,
    Upgrade = 11,
    Production = 12,
    Map = 13,
    Special = 14,
    Potion = 15,
    Event = 16,
    Title = 17,
    Quest1 = 18,
    Sell = 20,
    Food = 21,
    Snack = 22,
    Magical = 24,
    Part = 25,
    Teacher = 26,
    Ammo = 27,
    Quest2 = 28,
    House = 30,
    Garden = 31,
    Minigame = 32,
    Terrace = 33,
    MinilandTheme = 34
}