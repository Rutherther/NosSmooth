//
//  WalkInRangeOperation.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Extensions.Combat.Errors;
using NosSmooth.Extensions.Pathfinding;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Operations;

/// <summary>
/// A combat operation that walks into a given range of an entity.
/// </summary>
/// <param name="WalkManager">The walk manager.</param>
/// <param name="Entity">The entity to walk to.</param>
/// <param name="Distance">The maximal distance from the entity.</param>
public record WalkInRangeOperation
(
    WalkManager WalkManager,
    IEntity Entity,
    float Distance
) : ICombatOperation
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
        var character = combatState.Game.Character;
        if (character is null)
        {
            return new CharacterNotInitializedError();
        }

        var distance = Distance;
        while (distance >= 1)
        {
            var position = Entity.Position;
            if (position is null)
            {
                return new GenericError("Entity's position is not initialized.");
            }

            var currentPosition = character.Position;
            if (currentPosition is null)
            {
                return new CharacterNotInitializedError("Position");
            }

            var closePosition = GetClosePosition(currentPosition.Value, position.Value, distance);
            var walkResult = await WalkManager.GoToAsync(closePosition.X, closePosition.Y, ct);
            if (!walkResult.IsSuccess && walkResult.Error is NotFoundError)
            {
                distance--;
                continue;
            }

            return walkResult;
        }

        return Result.FromSuccess();
    }

    private Position GetClosePosition(Position start, Position target, double distance)
    {
        var diff = start - target;
        var diffLength = Math.Sqrt(diff.DistanceSquared(Position.Zero));
        return target + ((distance / diffLength) * diff);
    }
}