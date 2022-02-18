//
//  ICombatOperation.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Extensions.Combat.Techniques;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Operations;

/// <summary>
/// A combat operation used in <see cref="ICombatTechnique"/> that can be used as one step.
/// </summary>
public interface ICombatOperation
{
    /// <summary>
    /// Checks whether the operation can currently be used.
    /// </summary>
    /// <remarks>
    /// Ie. if the operation is to use a skill, it will return true only if the skill is not on a cooldown,
    /// the character has enough mana and is not stunned.
    /// </remarks>
    /// <param name="combatState">The combat state.</param>
    /// <returns>Whether the operation can be used right away.</returns>
    public Result<CanBeUsedResponse> CanBeUsed(ICombatState combatState);

    /// <summary>
    /// Use the operation, if possible.
    /// </summary>
    /// <remarks>
    /// Should block until the operation is finished.
    /// </remarks>
    /// <param name="combatState">The combat state.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not succeed.</returns>
    public Task<Result> UseAsync(ICombatState combatState, CancellationToken ct = default);
}