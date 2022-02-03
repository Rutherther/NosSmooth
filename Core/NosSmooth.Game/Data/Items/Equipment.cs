//
//  Equipment.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Items;

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