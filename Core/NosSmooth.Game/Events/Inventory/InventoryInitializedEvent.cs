//
//  InventoryInitializedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Events.Inventory;

/// <summary>
/// The whole inventory was initialized (every bag)
/// </summary>
/// <param name="Inventory">The game inventory.</param>
public record InventoryInitializedEvent
(
    Data.Inventory.Inventory Inventory
) : IGameEvent;