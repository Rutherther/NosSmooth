//
//  ICombatTechnique.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Extensions.Combat.Operations;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Techniques;

/// <summary>
/// A combat technique that allows to handle the whole combat situations using step callbacks.
/// </summary>
/// <remarks>
/// The callback methods decide the next steps, used in <see cref="CombatManager"/>.
/// </remarks>
public interface ICombatTechnique
{
    /// <summary>
    /// Gets the types this technique may handle.
    /// </summary>
    /// <remarks>
    /// <see cref="HandleNextCombatStep"/> will be called only for queue types
    /// from this collection.
    /// </remarks>
    public IReadOnlyList<OperationQueueType> HandlingQueueTypes { get; }

    /// <summary>
    /// Should check whether the technique should process more steps or quit the combat.
    /// </summary>
    /// <param name="state">The combat state.</param>
    /// <returns>Whether to continue with steps.</returns>
    public bool ShouldContinue(ICombatState state);

    /// <summary>
    /// Handle one step that should enqueue an operation.
    /// Enqueue only operation of the given queue type.
    /// </summary>
    /// <remarks>
    /// If error is returned, the combat will be cancelled.
    /// </remarks>
    /// <param name="queueType">The type of the operation to enqueue.</param>
    /// <param name="state">The combat state.</param>
    /// <returns>An id of the current target entity or an error.</returns>
    public Result<long?> HandleNextCombatStep(OperationQueueType queueType, ICombatState state);

    /// <summary>
    /// Handle waiting for an operation.
    /// </summary>
    /// <param name="queueType">The type of the operation.</param>
    /// <param name="state">The combat state.</param>
    /// <param name="operation">The operation that needs waiting.</param>
    /// <param name="error">The error received from the operation.</param>
    /// <returns>A result that may or may not have succeeded. In case of an error, <see cref="HandleError"/> will be called with the error.</returns>
    public Result HandleWaiting(OperationQueueType queueType, ICombatState state, ICombatOperation operation, CannotBeUsedError error);

    /// <summary>
    /// Handles an arbitrary error.
    /// </summary>
    /// <remarks>
    /// If an error is returned, the combat will be cancelled.
    /// </remarks>
    /// <param name="state">The combat state.</param>
    /// <param name="result">The errorful result.</param>
    /// <returns>A result that may or may not succeed.</returns>
    public Result HandleError(ICombatState state, Result result);
}