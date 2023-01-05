//
//  InventoryBag.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Concurrent;
using NosSmooth.Data.Abstractions.Enums;

namespace NosSmooth.Game.Data.Inventory;

/// <summary>
/// Represents one bag in the inventory of the player.
/// </summary>
public class InventoryBag : IEnumerable<InventorySlot>
{
    private ConcurrentDictionary<short, InventorySlot> _slots;

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryBag"/> class.
    /// </summary>
    /// <param name="bagType">The type of the bag.</param>
    /// <param name="slots">The number of slots.</param>
    public InventoryBag(BagType bagType, short slots)
    {
        Type = bagType;
        Slots = slots;
        _slots = new ConcurrentDictionary<short, InventorySlot>();
    }

    /// <summary>
    /// Gets the type of teh bag.
    /// </summary>
    public BagType Type { get; }

    /// <summary>
    /// Gets the number of slots.
    /// </summary>
    public short Slots { get; internal set; }

    /// <summary>
    /// Get contents of the given slot.
    /// </summary>
    /// <param name="slot">The slot to get contents of.</param>
    /// <returns>A slot.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The slot is outside of the bounds of the bag.</exception>
    public InventorySlot GetSlot(short slot)
    {
        if (slot < 0 || slot >= Slots)
        {
            throw new ArgumentOutOfRangeException(nameof(slot), slot, "There is not that many slots in the bag.");
        }

        return _slots.GetValueOrDefault(slot, new InventorySlot(slot, 0, null));
    }

    /// <summary>
    /// Clears the bag.
    /// </summary>
    internal void Clear()
    {
        _slots.Clear();
    }

    /// <summary>
    /// Sets the given slot item.
    /// </summary>
    /// <param name="slot">The slot information to set.</param>
    internal void SetSlot(InventorySlot slot)
    {
        if (slot.Item is null)
        {
            _slots.Remove(slot.Slot, out _);
        }
        else
        {
            if (slot.Slot >= Slots)
            {
                Slots = (short)(slot.Slot + 1);
            }

            _slots[slot.Slot] = slot;
        }
    }

    /// <summary>
    /// Gets a slot by index.
    /// </summary>
    /// <param name="slot">The slot.</param>
    public InventorySlot this[short slot] => GetSlot(slot);

    /// <inheritdoc />
    public IEnumerator<InventorySlot> GetEnumerator()
    {
        return _slots.Values.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}