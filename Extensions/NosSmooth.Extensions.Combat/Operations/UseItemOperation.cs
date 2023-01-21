//
//  UseItemOperation.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Extensions.Combat.Selectors;
using NosSmooth.Game.Data.Inventory;
using NosSmooth.Game.Extensions;
using NosSmooth.Packets.Client.Inventory;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Operations;

/// <summary>
/// A combat operation to use an item.
/// </summary>
/// <param name="Item">The item to use.</param>
public record UseItemOperation(InventoryItem Item) : ICombatOperation
{
    private Task<Result>? _useItemOperation;
    private CancellationTokenSource? _ct;

    /// <inheritdoc />
    public OperationQueueType QueueType => OperationQueueType.Item;

    /// <inheritdoc />
    public Task<Result> BeginExecution(ICombatState combatState, CancellationToken ct = default)
    {
        if (_useItemOperation is not null)
        {
            return Task.FromResult(Result.FromSuccess());
        }

        _ct = new CancellationTokenSource();
        _useItemOperation = Task.Run(
            () => combatState.Client.SendPacketAsync(new UseItemPacket(Item.Bag.Convert(), Item.Item.Slot), _ct.Token),
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

        BeginExecution(combatState, ct);
        if (_useItemOperation is null)
        {
            throw new UnreachableException();
        }

        try
        {
            return await _useItemOperation;
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
        => _useItemOperation is not null && !IsFinished();

    /// <inheritdoc />
    public bool IsFinished()
        => _useItemOperation?.IsCompleted ?? false;

    /// <inheritdoc />
    public Result CanBeUsed(ICombatState combatState)
        => Result.FromSuccess();

    /// <inheritdoc />
    public void Dispose()
    {
        _ct?.Cancel();
        _useItemOperation?.Dispose();
    }
}