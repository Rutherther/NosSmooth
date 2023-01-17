//
//  CombatStateExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Extensions.Combat.Errors;
using NosSmooth.Extensions.Combat.Operations;
using NosSmooth.Extensions.Combat.Policies;
using NosSmooth.Extensions.Combat.Selectors;
using NosSmooth.Extensions.Pathfinding;
using NosSmooth.Game.Apis.Safe;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Items;
using OneOf.Types;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Extensions;

/// <summary>
/// Extension methods for <see cref="ICombatState"/>.
/// </summary>
public static class CombatStateExtensions
{
    /// <summary>
    /// Walk in the range of the given entity.
    /// </summary>
    /// <param name="state">The combat state.</param>
    /// <param name="walkManager">The walk manager.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="range">The range distance to walk to.</param>
    public static void WalkInRange
    (
        this ICombatState state,
        WalkManager walkManager,
        IEntity entity,
        float range
    )
    {
        state.EnqueueOperation(new WalkInRangeOperation(walkManager, entity, range));
    }

    /// <summary>
    /// Walk to the given position.
    /// </summary>
    /// <param name="combatState">The combat state.</param>
    /// <param name="walkManager">The walk manager.</param>
    /// <param name="x">The target x coordinate.</param>
    /// <param name="y">The target y coordinate.</param>
    public static void WalkTo
    (
        this ICombatState combatState,
        WalkManager walkManager,
        short x,
        short y
    )
    {
        combatState.EnqueueOperation(new WalkOperation(walkManager, x, y));
    }

    /// <summary>
    /// Use the given skill.
    /// </summary>
    /// <param name="combatState">The combat state.</param>
    /// <param name="skillsApi">The skills api for using a skill.</param>
    /// <param name="skill">The skill.</param>
    /// <param name="target">The target to use skill at.</param>
    public static void UseSkill(this ICombatState combatState, NostaleSkillsApi skillsApi, Skill skill, ILivingEntity target)
    {
        if (combatState.Game.Character is null)
        {
            throw new InvalidOperationException("The character is not initialized.");
        }

        combatState.EnqueueOperation(new UseSkillOperation(skillsApi, skill, combatState.Game.Character, target));
    }

    /// <summary>
    /// Use the given item.
    /// </summary>
    /// <param name="combatState">The combat state.</param>
    /// <param name="item">The item to use.</param>
    public static void UseItem(this ICombatState combatState, InventoryItem item)
    {
        combatState.EnqueueOperation(new UseItemOperation(item));
    }
}