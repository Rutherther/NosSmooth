//
//  InventorySlotUpdatedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Game.Data.Inventory;

namespace NosSmooth.Game.Events.Inventory;

/// <summary>
/// A solt inside of inventory bag was updated.
/// </summary>
/// <param name="Bag">The updated bag.</param>
/// <param name="Slot">The updated slot.</param>
public record InventorySlotUpdatedEvent
(
    InventoryBag Bag,
    InventorySlot Slot
) : IGameEvent;