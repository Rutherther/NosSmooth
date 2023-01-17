//
//  ICombatState.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
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
    /// Gets whether there is an operation that cannot be used
    /// and we must wait for it to be usable.
    /// </summary>
    public bool IsWaitingOnOperation { get; }

    /// <summary>
    /// Get the operations the state is waiting for to to be usable.
    /// </summary>
    /// <returns>The operations needed to wait for.</returns>
    public IReadOnlyList<ICombatOperation> GetWaitingForOperations();

    /// <summary>
    /// Gets the current operation of the given queue type.
    /// </summary>
    /// <param name="queueType">The queue type to get the current operation of.</param>
    /// <returns>The operation of the given queue, if any.</returns>
    public ICombatOperation? GetCurrentOperation(OperationQueueType queueType);

    /// <summary>
    /// Checks whether an operation is being executed in the given queue.
    /// </summary>
    /// <remarks>
    /// If not, either waiting for the operation or there is no operation enqueued.
    /// </remarks>
    /// <param name="queueType">The type of queue to look at.</param>
    /// <param name="operation">The operation currently being executed.</param>
    /// <returns>Whether an operation is being executed.</returns>
    public bool IsExecutingOperation(OperationQueueType queueType, [NotNullWhen(true)] out ICombatOperation? operation);

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