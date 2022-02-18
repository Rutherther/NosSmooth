//
//  EnemyPolicy.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Data.SqlTypes;
using NosSmooth.Extensions.Combat.Errors;
using NosSmooth.Extensions.Combat.Selectors;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Policies;

/// <summary>
/// An enemy selector policy that selects the monsters based on their vnums.
/// </summary>
/// <param name="SelectPolicy">The policy to select an enemy.</param>
/// <param name="MonsterVNums">The vnums of the monsters to target.</param>
/// <param name="CombatArea">The area in which to get enemies from.</param>
public record EnemyPolicy
(
    EnemySelectPolicy SelectPolicy,
    int[]? MonsterVNums = default,
    CombatArea? CombatArea = default
) : IEnemySelector
{
    /// <inheritdoc />
    public Result<ILivingEntity> GetSelectedEntity(ICombatState combatState, ICollection<ILivingEntity> possibleTargets)
    {
        var targets = possibleTargets.OfType<Monster>();
        if (MonsterVNums is not null)
        {
            targets = targets.Where(x => MonsterVNums.Contains(x.VNum));
        }

        if (!targets.Any())
        {
            return new EntityNotFoundError();
        }

        if (combatState.Game.Character is null)
        {
            return new CharacterNotInitializedError();
        }

        var position = combatState.Game.Character.Position;
        if (position is null)
        {
            return new CharacterNotInitializedError();
        }

        var characterPosition = position.Value;
        ILivingEntity? target = null;
        switch (SelectPolicy)
        {
            case EnemySelectPolicy.Aggressive:
                throw new NotImplementedException(); // TODO: implement aggressive policy
            case EnemySelectPolicy.Closest:
                target = targets
                    .Where(x => x.Position is not null && (CombatArea?.IsInside(x.Position.Value) ?? true))
                    .MinBy(x => x.Position!.Value.DistanceSquared(characterPosition))!;
                break;
            case EnemySelectPolicy.LowestHealth:
                target = targets.MinBy
                (
                    x =>
                    {
                        if (x.Hp is null)
                        {
                            return int.MaxValue;
                        }

                        if (x.Hp.Amount is not null)
                        {
                            return x.Hp.Amount;
                        }

                        if (x.Hp.Percentage is not null && x.Level is not null)
                        {
                            return x.Hp.Percentage * 100 * x.Level;
                        }

                        if (x.Hp.Maximum is not null)
                        {
                            return x.Hp.Maximum; // Assume max health, best guess.
                        }

                        return int.MaxValue;
                    }
                );
                break;
        }

        if (target is null)
        {
            return new EntityNotFoundError();
        }

        return Result<ILivingEntity>.FromSuccess(target);
    }
}

/// <summary>
/// A policy enemy selector.
/// </summary>
public enum EnemySelectPolicy
{
    /// <summary>
    /// Select the enemy with the lowest health.
    /// </summary>
    LowestHealth,

    /// <summary>
    /// Selects the enemy that targets the user.
    /// </summary>
    Aggressive,

    /// <summary>
    /// Selects the enemy that is the closest to the character.
    /// </summary>
    Closest
}

/// <summary>
/// The combat area around which to find enemies.
/// </summary>
/// <param name="CenterX">The area center x coordinate.</param>
/// <param name="CenterY">The area center y coordinate.</param>
/// <param name="Range">The maximum range from the center.</param>
public record CombatArea(short CenterX, short CenterY, short Range)
{
    /// <summary>
    /// Create a combat area around a specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="range">The range.</param>
    /// <returns>The combat area.</returns>
    /// <exception cref="ArgumentException">If the entity does not have a position.</exception>
    public static CombatArea CreateAroundEntity(IEntity entity, short range)
    {
        var position = entity.Position;
        if (position is null)
        {
            throw new ArgumentException(nameof(entity));
        }

        return new CombatArea(position.Value.X, position.Value.Y, range);
    }

    /// <summary>
    /// Gets whether the position is inside of the combat area.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns>Whether the position is inside.</returns>
    public bool IsInside(Position position)
    {
        return position.DistanceSquared(new Position(CenterX, CenterY)) < Range * Range;
    }
}