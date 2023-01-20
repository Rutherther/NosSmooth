//
//  SimpleAttackTechnique.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Extensions.Combat.Errors;
using NosSmooth.Extensions.Combat.Extensions;
using NosSmooth.Extensions.Combat.Operations;
using NosSmooth.Extensions.Combat.Selectors;
using NosSmooth.Extensions.Pathfinding;
using NosSmooth.Game.Apis.Safe;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Inventory;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Techniques;

/// <summary>
/// A combat technique that will attack on the specified enemy, walk within range and use skill until the enemy is dead.
/// </summary>
public class SimpleAttackTechnique : ICombatTechnique
{
    private static OperationQueueType[] _handlingTypes = new[]
    {
        OperationQueueType.Item, OperationQueueType.TotalControl
    };

    private readonly long _targetId;
    private readonly NostaleSkillsApi _skillsApi;
    private readonly WalkManager _walkManager;
    private readonly ISkillSelector _skillSelector;
    private readonly IItemSelector _itemSelector;

    private ILivingEntity? _target;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleAttackTechnique"/> class.
    /// </summary>
    /// <param name="targetId">The target entity id.</param>
    /// <param name="skillsApi">The skills api.</param>
    /// <param name="walkManager">The walk manager.</param>
    /// <param name="skillSelector">The skill selector.</param>
    /// <param name="itemSelector">The item selector.</param>
    public SimpleAttackTechnique
    (
        long targetId,
        NostaleSkillsApi skillsApi,
        WalkManager walkManager,
        ISkillSelector skillSelector,
        IItemSelector itemSelector
    )
    {
        _targetId = targetId;
        _skillsApi = skillsApi;
        _walkManager = walkManager;
        _skillSelector = skillSelector;
        _itemSelector = itemSelector;
    }

    /// <inheritdoc />
    public IReadOnlyList<OperationQueueType> HandlingQueueTypes => _handlingTypes;

    /// <inheritdoc />
    public bool ShouldContinue(ICombatState state)
    {
        var map = state.Game.CurrentMap;
        if (map is null)
        {
            return false;
        }

        if (_target is null)
        {
            _target = map.Entities.GetEntity<ILivingEntity>(_targetId);
        }

        return !(_target is null || (_target.Hp is not null && (_target.Hp.Amount <= 0 || _target.Hp.Percentage <= 0)));
    }

    /// <inheritdoc />
    public Result HandleWaiting(OperationQueueType queueType, ICombatState state, ICombatOperation operation)
    { // does not do anything, just wait.
        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public Result HandleError(ICombatState state, Result result)
    { // no handling of errors is done
        return result;
    }

    private Result<long?> HandleTotalControl(ICombatState state)
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

        var skills = state.Game.Skills;
        if (skills is null)
        {
            return new CharacterNotInitializedError("Skills");
        }

        var characterMp = character.Mp?.Amount ?? 0;
        var usableSkills = skills.AllSkills
            .Where
            (
                x => x.Info is not null && x.Info.HitType != HitType.AlliesInZone
                    && x.Info.SkillType == SkillType.Player
            )
            .Where(x => !x.IsOnCooldown && characterMp >= (x.Info?.MpCost ?? long.MaxValue));

        var skillResult = _skillSelector.GetSelectedSkill(usableSkills);
        if (!skillResult.IsDefined(out var currentSkill))
        {
            if (skillResult.Error is SkillNotFoundError)
            {
                return _target.Id;
            }

            return Result<long?>.FromError(skillResult);
        }

        if (currentSkill.Info is null)
        {
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

        var range = currentSkill.Info.Range;
        if (currentSkill.Info.TargetType == TargetType.Self && currentSkill.Info.HitType == HitType.EnemiesInZone
            && currentSkill.Info.AttackType != AttackType.Dash)
        {
            range = currentSkill.Info.ZoneRange;
        }

        state.EnqueueOperation
        (
            new CompoundOperation
            (
                OperationQueueType.TotalControl,
                new WalkInRangeOperation(_walkManager, _target, range),
                new UseSkillOperation(_skillsApi, currentSkill, character, _target)
            )
        );
        return _target.Id;
    }

    private Result<long?> HandleItem(ICombatState state)
    {
        var shouldUseItemResult = _itemSelector.ShouldUseItem(state);
        if (!shouldUseItemResult.IsDefined(out var shouldUseItem))
        {
            return Result<long?>.FromError(shouldUseItemResult);
        }

        if (!shouldUseItem)
        {
            return _targetId;
        }

        var inventory = state.Game.Inventory;
        if (inventory is null)
        {
            return _targetId;
        }

        var main = inventory.GetBag(BagType.Main)
            .Where(x => x is { Amount: > 0, Item.Info.Type: ItemType.Potion })
            .Select(x => new InventoryItem(BagType.Main, x));
        var etc = inventory.GetBag(BagType.Etc)
            .Where(x => x is { Amount: > 0, Item.Info.Type: ItemType.Food or ItemType.Snack })
            .Select(x => new InventoryItem(BagType.Etc, x));

        var possibleItems = main.Concat(etc).ToList();

        var itemResult = _itemSelector.GetSelectedItem(state, possibleItems);

        if (!itemResult.IsDefined(out var item))
        {
            return Result<long?>.FromError(itemResult);
        }

        state.UseItem(item);
        return _targetId;
    }

    /// <inheritdoc />
    public Result<long?> HandleNextCombatStep(OperationQueueType queueType, ICombatState state)
    {
        switch (queueType)
        {
            case OperationQueueType.Item:
                return HandleItem(state);
            case OperationQueueType.TotalControl:
                return HandleTotalControl(state);
        }

        throw new InvalidOperationException("SimpleAttackTechnique supports only Item and TotalControl queue types.");
    }
}