//
//  UpgradeableItem.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Infos;

namespace NosSmooth.Game.Data.Items;

/// <summary>
/// An item that can be upgraded and has rarity, ie. weapon or armor.
/// </summary>
/// <param name="ItemVNum">The vnum of the item.</param>
/// <param name="Info">The information about the item.</param>
/// <param name="Upgrade">The upgrade (0 - 10).</param>
/// <param name="Rare">The rare nubmer (0 - 8).</param>
public record UpgradeableItem(int ItemVNum, IItemInfo? Info, byte? Upgrade, sbyte? Rare) : Item(ItemVNum, Info);