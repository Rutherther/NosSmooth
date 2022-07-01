//
//  IEnemySelector.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Entities;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Selectors;

/// <summary>
/// Selects an enemy from the possible enemies.
/// </summary>
public interface IEnemySelector
{
    /// <summary>
    /// Gets the entity to be currently selected.
    /// </summary>
    /// <param name="combatState">The combat state.</param>
    /// <param name="possibleTargets">The collection of possible targets.</param>
    /// <returns>The selected entity, or an error.</returns>
    public Result<ILivingEntity> GetSelectedEntity(ICombatState combatState, ICollection<ILivingEntity> possibleTargets);
}