//
//  ICombatState.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
using NosSmooth.Extensions.Combat.Operations;
using NosSmooth.Game.Data.Entities;

namespace NosSmooth.Extensions.Combat;

/// <summary>
/// The combat technique state used for queuing operations and storing information.
/// </summary>
public interface ICombatState
{
    /// <summary>
    /// Gets the combat manager.
    /// </summary>
    public CombatManager CombatManager { get; }

    /// <summary>
    /// Gets the game.
    /// </summary>
    public Game.Game Game { get; }

    /// <summary>
    /// Gets the NosTale client.
    /// </summary>
    public INostaleClient Client { get; }

    /// <summary>
    /// Cancel the combat technique, quit the combat state.
    /// </summary>
    public void QuitCombat();

    /// <summary>
    /// Replace the current operation with this one.
    /// </summary>
    /// <param name="operation">The operation to use.</param>
    /// <param name="emptyQueue">Whether to empty the queue of the operations.</param>
    /// <param name="prependCurrentOperationToQueue">Whether to still use the current operation (true) after this one or discard it (false).</param>
    public void SetCurrentOperation
        (ICombatOperation operation, bool emptyQueue = false, bool prependCurrentOperationToQueue = false);

    /// <summary>
    /// Enqueue the operation at the end of the queue.
    /// </summary>
    /// <param name="operation">The operation to enqueue.</param>
    public void EnqueueOperation(ICombatOperation operation);

    /// <summary>
    /// Remove the operations by the given filter.
    /// </summary>
    /// <param name="filter">Called for each operation, should return true if it should be removed.</param>
    public void RemoveOperations(Func<ICombatOperation, bool> filter);
}