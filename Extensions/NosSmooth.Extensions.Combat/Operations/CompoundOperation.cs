//
//  CompoundOperation.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using NosSmooth.Extensions.Combat.Techniques;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Operations;

/// <summary>
/// An operation made from multiple operations
/// that should execute right after each other.
/// </summary>
public class CompoundOperation : ICombatOperation
{
    private readonly ICombatTechnique _technique;
    private readonly ICombatOperation[] _operations;
    private readonly OperationQueueType _queueType;
    private ICombatOperation? _currentOperation;
    private CancellationTokenSource? _ct;
    private Task<Result>? _compoundOperation;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompoundOperation"/> class.
    /// </summary>
    /// <param name="technique">The combat technique used for calling HandleWaiting.</param>
    /// <param name="queueType">The queue type.</param>
    /// <param name="operations">The operations to execute.</param>
    public CompoundOperation
        (ICombatTechnique technique, OperationQueueType queueType = OperationQueueType.TotalControl, params ICombatOperation[] operations)
    {
        if (operations.Length == 0)
        {
            throw new ArgumentNullException(nameof(operations), "The compound operation needs at least one operation.");
        }

        _technique = technique;
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
    public OperationQueueType QueueType => _queueType;

    /// <inheritdoc />
    public bool MayBeCancelled => _currentOperation?.MayBeCancelled ?? true;

    /// <inheritdoc />
    public Task<Result> BeginExecution(ICombatState combatState, CancellationToken ct = default)
    {
        if (_compoundOperation is not null)
        {
            return Task.FromResult(Result.FromSuccess());
        }

        _ct = new CancellationTokenSource();
        _compoundOperation = Task.Run
        (
            () => UseAsync(combatState, _ct.Token),
            _ct.Token
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

        try
        {
            return await _compoundOperation;
        }
        catch (OperationCanceledException)
        {
            return Result.FromSuccess();
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <inheritdoc />
    public bool IsExecuting()
        => _compoundOperation is not null && !IsFinished();

    /// <inheritdoc />
    public bool IsFinished()
        => _compoundOperation?.IsCompleted ?? false;

    /// <inheritdoc />
    public Result CanBeUsed(ICombatState combatState)
        => _operations[0].CanBeUsed(combatState);

    /// <inheritdoc />
    public void Cancel()
    {
        _currentOperation?.Cancel();
        _ct?.Cancel();
    }

    private async Task<Result> UseAsync(ICombatState combatState, CancellationToken ct)
    {
        foreach (var operation in _operations)
        {
            _currentOperation = operation;
            CanBeUsedResponse canBeUsed = CanBeUsedResponse.MustWait;

            while (canBeUsed != CanBeUsedResponse.CanBeUsed)
            {
                var canBeUsedResult = operation.CanBeUsed(combatState);
                if (canBeUsedResult is { IsSuccess: false, Error: not CannotBeUsedError })
                {
                    return canBeUsedResult;
                }

                var error = canBeUsedResult.Error as CannotBeUsedError;
                canBeUsed = error?.Response ?? CanBeUsedResponse.CanBeUsed;

                if (canBeUsed != CanBeUsedResponse.CanBeUsed)
                {
                    _technique.HandleWaiting(QueueType, combatState, this, error!);
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