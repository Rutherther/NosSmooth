//
//  EquipmentSlot.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
#pragma warning disable CS1591

namespace NosSmooth.Data.Abstractions.Enums;

/// <summary>
/// An equipment slot that wearable item goes to.
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Self-explanatory.")]
public enum EquipmentSlot
{
    MainWeapon = 0,
    Armor = 1,
    Hat = 2,
    Gloves = 3,
    Boots = 4,
    SecondaryWeapon = 5,
    Necklace = 6,
    Ring = 7,
    Bracelet = 8,
    Mask = 9,
    Fairy = 10,
    Amulet = 11,
    Sp = 12,
    CostumeSuit = 13,
    CostumeHat = 14,
    WeaponSkin = 15,
    Wings = 16
}