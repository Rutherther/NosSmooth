//
//  SpItem.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Infos;

namespace NosSmooth.Game.Data.Items;

/// <summary>
/// A special card NosTale item.
/// </summary>
/// <param name="ItemVNum">The item's VNum.</param>
/// <param name="Info">The item's info.</param>
/// <param name="Rare">Unknown TODO.</param>
/// <param name="Upgrade">The upgrade (+) of the sp.</param>
/// <param name="SpStone">The number of sp stones.</param>
public record SpItem
(
    int ItemVNum,
    IItemInfo? Info,
    sbyte? Rare,
    byte? Upgrade,
    byte? SpStone
) : Item(ItemVNum, Info);