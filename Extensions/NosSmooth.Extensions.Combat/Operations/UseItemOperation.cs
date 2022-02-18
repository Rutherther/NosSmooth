//
//  UseItemOperation.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Items;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Operations;

// TODO: first inventory has to be made
public record UseItemOperation(Item Item) : ICombatOperation
{
    /// <inheritdoc />
    public Result<CanBeUsedResponse> CanBeUsed(ICombatState combatState)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result> UseAsync(ICombatState combatState, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}