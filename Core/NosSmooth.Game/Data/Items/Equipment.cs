//
//  Equipment.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Items;

/// <summary>
/// Information about character's equipment.
/// </summary>
/// <param name="Hat">The hat.</param>
/// <param name="Armor">The armor.</param>
/// <param name="MainWeapon">The main weapon.</param>
/// <param name="SecondaryWeapon">The secondary weapon.</param>
/// <param name="Mask">The mask.</param>
/// <param name="Fairy">The fairy.</param>
/// <param name="CostumeSuit">The costume suit.</param>
/// <param name="CostumeHat">The costume hat.</param>
/// <param name="WeaponSkin">The weapon skin.</param>
/// <param name="WingSkin">The wing skin.</param>
public record Equipment
(
     Item? Hat,
     UpgradeableItem? Armor,
     UpgradeableItem? MainWeapon,
     UpgradeableItem? SecondaryWeapon,
     Item? Mask,
     Item? Fairy,
     Item? CostumeSuit,
     Item? CostumeHat,
     short? WeaponSkin,
     short? WingSkin
);