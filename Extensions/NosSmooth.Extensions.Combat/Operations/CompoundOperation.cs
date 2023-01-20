//
//  CompoundOperation.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Operations;

/// <summary>
/// An operation made from multiple operations
/// that should execute right after each other.
/// </summary>
public class CompoundOperation : ICombatOperation
{
    private readonly ICombatOperation[] _operations;
    private readonly OperationQueueType _queueType;
    private Task<Result>? _compoundOperation;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompoundOperation"/> class.
    /// </summary>
    /// <param name="operations">The operations to execute.</param>
    /// <param name="queueType">The queue type.</param>
    public CompoundOperation
        (OperationQueueType queueType = OperationQueueType.TotalControl, params ICombatOperation[] operations)
    {
        if (operations.Length == 0)
        {
            throw new ArgumentNullException(nameof(operations), "The compound operation needs at least one operation.");
        }

        _operations = operations;
        _queueType = queueType;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var operation in _operations)
        {
            operation.Dispose();
        }
    }

    /// <inheritdoc />
    public OperationQueueType QueueType { get; }

    /// <inheritdoc />
    public Task<Result> BeginExecution(ICombatState combatState, CancellationToken ct = default)
    {
        if (_compoundOperation is not null)
        {
            return Task.FromResult(Result.FromSuccess());
        }

        _compoundOperation = Task.Run
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
        if (_compoundOperation is null)
        {
            throw new UnreachableException();
        }

        return await _compoundOperation;
    }

    /// <inheritdoc />
    public bool IsExecuting()
        => _compoundOperation is not null && !IsFinished();

    /// <inheritdoc />
    public bool IsFinished()
        => _compoundOperation?.IsCompleted ?? false;

    /// <inheritdoc />
    public Result<CanBeUsedResponse> CanBeUsed(ICombatState combatState)
        => _operations[0].CanBeUsed(combatState);

    private async Task<Result> UseAsync(ICombatState combatState, CancellationToken ct)
    {
        foreach (var operation in _operations)
        {
            CanBeUsedResponse canBeUsed = CanBeUsedResponse.MustWait;

            while (canBeUsed != CanBeUsedResponse.CanBeUsed)
            {
                var canBeUsedResult = operation.CanBeUsed(combatState);
                if (!canBeUsedResult.IsDefined(out canBeUsed))
                {
                    return Result.FromError(canBeUsedResult);
                }

                if (canBeUsed == CanBeUsedResponse.WontBeUsable)
                {
                    return new GenericError("Won't be usable.");
                }

                await Task.Delay(10, ct);
            }

            var result = await operation.BeginExecution(combatState, ct);
            if (!result.IsSuccess)
            {
                return result;
            }

            result = await operation.WaitForFinishedAsync(combatState, ct);
            if (!result.IsSuccess)
            {
                return result;
            }
        }

        return Result.FromSuccess();
    }
}