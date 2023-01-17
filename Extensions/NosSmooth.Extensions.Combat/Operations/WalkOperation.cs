//
//  WalkOperation.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using NosSmooth.Extensions.Combat.Errors;
using NosSmooth.Extensions.Pathfinding;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Operations;

/// <summary>
/// A combat operation that walks to the target.
/// </summary>
/// <param name="WalkManager">The walk manager.</param>
/// <param name="X">The x coordinate to walk to.</param>
/// <param name="Y">The y coordinate to walk to.</param>
public record WalkOperation(WalkManager WalkManager, short X, short Y) : ICombatOperation
{
    private Task<Result>? _walkOperation;

    /// <inheritdoc />
    public OperationQueueType QueueType => OperationQueueType.TotalControl;

    /// <inheritdoc />
    public Task<Result> BeginExecution(ICombatState combatState, CancellationToken ct = default)
    {
        if (_walkOperation is not null)
        {
            return Task.FromResult(Result.FromSuccess());
        }

        _walkOperation = Task.Run
        (
            () => UseAsync(combatState, ct),
            ct
        );
        return Task.FromResult(Result.FromSuccess());
    }

    /// <inheritdoc />
    public async Task<Result> WaitForFinishedAsync(ICombatState combatState, CancellationToken ct = default)
    {
        if (IsFinished())
        {
            return Result.FromSuccess();
        }

        await BeginExecution(combatState, ct);
        if (_walkOperation is null)
        {
            throw new UnreachableException();
        }

        return await _walkOperation;
    }

    /// <inheritdoc />
    public bool IsExecuting()
        => _walkOperation is not null && !IsFinished();

    /// <inheritdoc />
    public bool IsFinished()
        => _walkOperation?.IsCompleted ?? false;

    /// <inheritdoc />
    public Result<CanBeUsedResponse> CanBeUsed(ICombatState combatState)
    {
        var character = combatState.Game.Character;
        if (character is null)
        {
            return new CharacterNotInitializedError();
        }

        return character.CantMove ? CanBeUsedResponse.MustWait : CanBeUsedResponse.CanBeUsed;
    }

    private Task<Result> UseAsync(ICombatState combatState, CancellationToken ct = default)
        => WalkManager.GoToAsync(X, Y, true, ct);

    /// <inheritdoc />
    public void Dispose()
    {
        _walkOperation?.Dispose();
    }
}