//
//  ICombatOperation.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Contracts;
using NosSmooth.Extensions.Combat.Techniques;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Operations;

/// <summary>
/// A combat operation used in <see cref="ICombatTechnique"/> that can be used as one step.
/// </summary>
public interface ICombatOperation : IDisposable
{
    /// <summary>
    /// Gets the queue type the operation belongs to.
    /// </summary>
    /// <remarks>
    /// Used for distinguishing what operations may run simultaneously.
    /// For example items may be used simultaneous to attacking. Attacking
    /// may not be simultaneous to walking.
    /// </remarks>
    public OperationQueueType QueueType { get; }

    /// <summary>
    /// Begin the execution without waiting for the finished state.
    /// </summary>
    /// <param name="combatState">The combat state.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> BeginExecution(ICombatState combatState, CancellationToken ct = default);

    /// <summary>
    /// Asynchronously wait for finished state.
    /// </summary>
    /// <param name="combatState">The combat state.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<Result> WaitForFinishedAsync(ICombatState combatState, CancellationToken ct = default);

    /// <summary>
    /// Checks whether the operation is currently being executed.
    /// </summary>
    /// <returns>Whether the operation is being executed.</returns>
    public bool IsExecuting();

    /// <summary>
    /// Checks whether the operation is done.
    /// </summary>
    /// <returns>Whether the operation is finished.</returns>
    public bool IsFinished();

    /// <summary>
    /// Checks whether the operation can currently be used.
    /// </summary>
    /// <remarks>
    /// Ie. if the operation is to use a skill, it will return true only if the skill is not on a cooldown,
    /// the character has enough mana and is not stunned.
    /// </remarks>
    /// <param name="combatState">The combat state.</param>
    /// <returns>Whether the operation can be used right away.</returns>
    public Result CanBeUsed(ICombatState combatState);
}