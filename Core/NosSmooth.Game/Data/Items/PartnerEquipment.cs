//
//  PartnerEquipment.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Items;

/// <summary>
/// An equipment of a partner.
/// </summary>
/// <param name="Weapon">The weapon of the partner.</param>
/// <param name="Armor">The armor of the partner.</param>
/// <param name="Gauntlet">The gauntlet of the partner.</param>
/// <param name="Boots">The boots of the partner.</param>
public record PartnerEquipment
(
    UpgradeableItem? Weapon,
    UpgradeableItem? Armor,
    UpgradeableItem? Gauntlet,
    UpgradeableItem? Boots
);