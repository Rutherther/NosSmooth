//
//  Inventory.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Concurrent;
using NosSmooth.Data.Abstractions.Enums;

namespace NosSmooth.Game.Data.Inventory;

/// <summary>
/// Represents the whole inventory of the character.
/// </summary>
public class Inventory : IEnumerable<InventoryBag>
{
    /// <summary>
    /// Get default number of slots in the given bag type.
    /// </summary>
    /// <param name="bag">The bag.</param>
    /// <returns>Default number of slots.</returns>
    public static short GetDefaultSlots(BagType bag)
    {
        switch (bag)
        {
            case BagType.Miniland:
                return 5 * 10;
            case BagType.Main:
            case BagType.Etc:
            case BagType.Equipment:
                return 6 * 8;
            case BagType.Costume:
            case BagType.Specialist:
                return 4 * 5;
            default:
                return 0;
        }
    }

    private readonly ConcurrentDictionary<BagType, InventoryBag> _bags;

    /// <summary>
    /// Initializes a new instance of the <see cref="Inventory"/> class.
    /// </summary>
    public Inventory()
    {
        _bags = new ConcurrentDictionary<BagType, InventoryBag>();
    }

    /// <summary>
    /// Gets a bag from inventory.
    /// </summary>
    /// <param name="bag">The bag.</param>
    /// <returns>An inventory bag.</returns>
    public InventoryBag GetBag(BagType bag)
        => _bags.GetOrAdd(bag, _ => new InventoryBag(bag, GetDefaultSlots(bag)));

    /// <summary>
    /// Gets a bag from inventory.
    /// </summary>
    /// <param name="bag">An inventory bag.</param>
    public InventoryBag this[BagType bag] => GetBag(bag);

    /// <inheritdoc />
    public IEnumerator<InventoryBag> GetEnumerator()
    {
        return _bags.Values.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}