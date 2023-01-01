//
//  UsePrimarySkillOperation.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Extensions.Combat.Errors;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Packets.Client.Battle;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Operations;

/// <summary>
/// An operation that uses the primary skill of the character.
/// </summary>
public record UsePrimarySkillOperation(ILivingEntity Target) : ICombatOperation
{
    private UseSkillOperation? _useSkillOperation;

    /// <inheritdoc />
    public Result<CanBeUsedResponse> CanBeUsed(ICombatState combatState)
    {
        if (_useSkillOperation is null)
        {
            var primarySkillResult = GetPrimarySkill(combatState);
            if (!primarySkillResult.IsDefined(out var primarySkill))
            {
                return Result<CanBeUsedResponse>.FromError(primarySkillResult);
            }

            if (combatState.Game.Character is null)
            {
                return new CharacterNotInitializedError();
            }

            _useSkillOperation = new UseSkillOperation(primarySkill, combatState.Game.Character, Target);
        }

        return _useSkillOperation.CanBeUsed(combatState);
    }

    /// <inheritdoc />
    public async Task<Result> UseAsync(ICombatState combatState, CancellationToken ct)
    {
        if (_useSkillOperation is null)
        {
            var primarySkillResult = GetPrimarySkill(combatState);
            if (!primarySkillResult.IsDefined(out var primarySkill))
            {
                return Result.FromError(primarySkillResult);
            }

            if (combatState.Game.Character is null)
            {
                return new CharacterNotInitializedError();
            }

            _useSkillOperation = new UseSkillOperation(primarySkill, combatState.Game.Character, Target);
        }

        return await _useSkillOperation.UseAsync(combatState, ct);
    }

    private Result<Skill> GetPrimarySkill(ICombatState combatState)
    {
        var character = combatState.Game.Character;
        if (character is null)
        {
            return new CharacterNotInitializedError();
        }

        var skills = character.Skills;
        if (skills is null)
        {
            return new CharacterNotInitializedError("Skills");
        }

        return skills.PrimarySkill;
    }
}