//
//  IItemSelector.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Items;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Selectors;

/// <summary>
/// Selects an item to be used.
/// </summary>
public interface IItemSelector
{
    /// <summary>
    /// Gets the entity to be currently selected.
    /// </summary>
    /// <param name="combatState">The combat state.</param>
    /// <param name="possibleItems">The items that may be used.</param>
    /// <returns>The selected item, or an error.</returns>
    public Result<InventoryItem> GetSelectedItem(ICombatState combatState, ICollection<InventoryItem> possibleItems);

    /// <summary>
    /// Gets whether currently an item should be used.
    /// </summary>
    /// <param name="combatState">The combat state.</param>
    /// <returns>Whether to use an item or an error.</returns>
    public Result<bool> ShouldUseItem(ICombatState combatState);
}