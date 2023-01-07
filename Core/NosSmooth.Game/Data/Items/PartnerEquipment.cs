//
//  PartnerEquipment.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Items;

public record PartnerEquipment
(
    UpgradeableItem? Weapon,
    UpgradeableItem? Armor,
    UpgradeableItem? Gauntlet,
    UpgradeableItem? Boots
);