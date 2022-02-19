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
    /// Should check whether the technique should process more steps or quit the combat.
    /// </summary>
    /// <param name="state">The combat state.</param>
    /// <returns>Whether to continue with steps.</returns>
    public bool ShouldContinue(ICombatState state);

    /// <summary>
    /// Handle one step that should enqueue an operation.
    /// </summary>
    /// <remarks>
    /// If error is returned, the combat will be cancelled.
    /// </remarks>
    /// <param name="state">The combat state.</param>
    /// <returns>An id of the current target entity or an error.</returns>
    public Result<long?> HandleCombatStep(ICombatState state);

    /// <summary>
    /// Handles an error from <see cref="ICombatOperation.UseAsync"/>.
    /// </summary>
    /// <remarks>
    /// If an error is returned, the combat will be cancelled.
    /// </remarks>
    /// <param name="state">The combat state.</param>
    /// <param name="operation">The combat operation that returned an error.</param>
    /// <param name="result">The errorful result.</param>
    /// <returns>A result that may or may not succeed.</returns>
    public Result HandleError(ICombatState state, ICombatOperation operation, Result result);
}