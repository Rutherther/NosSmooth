//
//  WalkOperation.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

    /// <inheritdoc />
    public async Task<Result> UseAsync(ICombatState combatState, CancellationToken ct = default)
    {
        return await WalkManager.GoToAsync(X, Y, true, ct);
    }
}