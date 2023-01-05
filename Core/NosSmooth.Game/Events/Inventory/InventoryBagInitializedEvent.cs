//
//  InventoryBagInitializedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Inventory;

namespace NosSmooth.Game.Events.Inventory;

/// <summary>
/// An inventory bag was initialized.
/// </summary>
/// <param name="Bag">The bag that was initialized.</param>
public record InventoryBagInitializedEvent
(
    InventoryBag Bag
) : IGameEvent;