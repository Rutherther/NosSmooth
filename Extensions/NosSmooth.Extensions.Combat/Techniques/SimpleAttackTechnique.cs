//
//  SimpleAttackTechnique.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Extensions.Combat.Errors;
using NosSmooth.Extensions.Combat.Extensions;
using NosSmooth.Extensions.Combat.Operations;
using NosSmooth.Extensions.Combat.Selectors;
using NosSmooth.Extensions.Pathfinding;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Entities;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Techniques;

/// <summary>
/// A combat technique that will attack on the specified enemy.
/// </summary>
public class SimpleAttackTechnique : ICombatTechnique
{
    private readonly int _targetId;
    private readonly WalkManager _walkManager;
    private readonly ISkillSelector _skillSelector;
    private readonly IItemSelector _itemSelector;

    private Skill? _currentSkill;
    private ILivingEntity? _target;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleAttackTechnique"/> class.
    /// </summary>
    /// <param name="targetId">The target entity id.</param>
    /// <param name="walkManager">The walk manager.</param>
    /// <param name="skillSelector">The skill selector.</param>
    /// <param name="itemSelector">The item selector.</param>
    public SimpleAttackTechnique
    (
        int targetId,
        WalkManager walkManager,
        ISkillSelector skillSelector,
        IItemSelector itemSelector
    )
    {
        _targetId = targetId;
        _walkManager = walkManager;
        _skillSelector = skillSelector;
        _itemSelector = itemSelector;
    }

    /// <inheritdoc />
    public bool ShouldContinue(ICombatState state)
    {
        var map = state.Game.CurrentMap;
        if (map is null)
        {
            return false;
        }

        var entity = map.Entities.GetEntity<ILivingEntity>(_targetId);
        return !(entity is null || (entity.Hp is not null && (entity.Hp.Amount <= 0 || entity.Hp.Percentage <= 0)));
    }

    /// <inheritdoc />
    public Result HandleCombatStep(ICombatState state)
    {
        var map = state.Game.CurrentMap;
        if (map is null)
        {
            return new MapNotInitializedError();
        }

        if (_target is null)
        {
            var entity = map.Entities.GetEntity<ILivingEntity>(_targetId);
            if (entity is null)
            {
                return new EntityNotFoundError();
            }

            _target = entity;
        }

        var character = state.Game.Character;
        if (character is null)
        {
            return new CharacterNotInitializedError();
        }

        if (_currentSkill is null)
        {
            var skills = character.Skills;
            if (skills is null)
            {
                return new CharacterNotInitializedError("Skills");
            }

            var characterMp = character.Mp?.Amount ?? 0;
            var usableSkills = new[] { skills.PrimarySkill, skills.SecondarySkill }
                .Concat(skills.OtherSkills)
                .Where(x => !x.IsOnCooldown && characterMp >= (x.Info?.MpCost ?? long.MaxValue));

            var skillResult = _skillSelector.GetSelectedSkill(usableSkills);
            if (!skillResult.IsSuccess)
            {
                if (skillResult.Error is SkillNotFoundError)
                {
                    return Result.FromSuccess();
                }

                return Result.FromError(skillResult);
            }

            _currentSkill = skillResult.Entity;
        }

        if (_currentSkill.Info is null)
        {
            var currentSkill = _currentSkill;
            _currentSkill = null;
            return new MissingInfoError("skill", currentSkill.SkillVNum);
        }

        if (character.Position is null)
        {
            return new CharacterNotInitializedError("Position");
        }

        if (_target.Position is null)
        {
            return new EntityNotFoundError();
        }

        if (!character.Position.Value.IsInRange(_target.Position.Value, _currentSkill.Info.Range))
        {
            state.WalkInRange(_walkManager, _target, _currentSkill.Info.Range);
        }
        else
        {
            state.UseSkill(_currentSkill, _target);
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public Result HandleError(ICombatState state, ICombatOperation operation, Result result)
    {
        return result;
    }
}