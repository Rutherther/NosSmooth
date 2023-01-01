//
//  WalkInRangeOperation.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Extensions.Combat.Errors;
using NosSmooth.Extensions.Pathfinding;
using NosSmooth.Extensions.Pathfinding.Errors;
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
        while (distance >= 0)
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

            if (Entity.Position?.DistanceSquared(currentPosition.Value) <= Distance * Distance)
            {
                return Result.FromSuccess();
            }

            var closePosition = GetClosePosition(currentPosition.Value, position.Value, distance);
            if (closePosition == currentPosition)
            {
                return Result.FromSuccess();
            }

            using var goToCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var walkResultTask = WalkManager.GoToAsync(closePosition.X, closePosition.Y, true, goToCancellationTokenSource.Token);

            while (!walkResultTask.IsCompleted)
            {
                await Task.Delay(5, ct);
                if (Entity.Position != position)
                {
                    goToCancellationTokenSource.Cancel();
                    await walkResultTask;
                }
            }

            if (Entity.Position != position)
            {
                continue;
            }

            var walkResult = await walkResultTask;
            if ((character.Position - Entity.Position)?.DistanceSquared(Position.Zero) <= Distance * Distance)
            {
                return Result.FromSuccess();
            }

            if (!walkResult.IsSuccess && walkResult.Error is PathNotFoundError)
            {
                if (distance - 1 > 0)
                {
                    distance--;
                }
                else
                {
                    distance = 0;
                }

                continue;
            }

            return walkResult;
        }

        return Result.FromSuccess();
    }

    private Position GetClosePosition(Position start, Position target, double distance)
    {
        var diff = start - target;
        if (diff.DistanceSquared(Position.Zero) < distance * distance)
        {
            return start;
        }

        var diffLength = Math.Sqrt(diff.DistanceSquared(Position.Zero));
        return target + ((distance / diffLength) * diff);
    }
}