//
//  UseSkillOperation.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using NosSmooth.Core.Contracts;
using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Extensions.Combat.Errors;
using NosSmooth.Game.Apis.Safe;
using NosSmooth.Game.Contracts;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Errors;
using NosSmooth.Game.Events.Battle;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Operations;

/// <summary>
/// A combat operation to use a skill.
/// </summary>
/// <param name="SkillsApi">The skills api used for executing the skills.</param>
/// <param name="Skill">The skill to use.</param>
/// <param name="Caster">The caster entity that is using the skill.</param>
/// <param name="Target">The target entity to use the skill at.</param>
public record UseSkillOperation
(
    NostaleSkillsApi SkillsApi,
    Skill Skill,
    ILivingEntity Caster,
    ILivingEntity Target
) : ICombatOperation
{
    private IContract<SkillUsedEvent, UseSkillStates>? _contract;

    /// <inheritdoc />
    public OperationQueueType QueueType => OperationQueueType.TotalControl;

    /// <inheritdoc />
    public async Task<Result> BeginExecution(ICombatState combatState, CancellationToken ct = default)
    {
        if (_contract is not null)
        {
            return Result.FromSuccess();
        }

        if (Skill.Info is null)
        {
            return new MissingInfoError("skill", Skill.SkillVNum);
        }

        if (Target.Position is null)
        {
            return new NotInitializedError("target's position");
        }

        var contractResult = ContractSkill(Skill.Info);
        if (!contractResult.IsDefined(out var contract))
        {
            return Result.FromError(contractResult);
        }

        _contract = contract;
        var executed = await _contract.OnlyExecuteAsync(ct);
        if (executed.IsSuccess)
        {
            _contract.Register();
        }

        return executed;
    }

    /// <inheritdoc />
    public async Task<Result> WaitForFinishedAsync(ICombatState combatState, CancellationToken ct = default)
    {
        var result = await BeginExecution(combatState, ct);
        if (!result.IsSuccess)
        {
            return result;
        }

        if (_contract is null)
        {
            throw new UnreachableException();
        }

        var waitResult = await _contract.WaitForAsync(UseSkillStates.CharacterRestored, ct: ct);
        return waitResult.IsSuccess ? Result.FromSuccess() : Result.FromError(waitResult);
    }

    /// <inheritdoc />
    public bool IsExecuting()
        => _contract is not null && _contract.CurrentState > UseSkillStates.None && !IsFinished();

    /// <inheritdoc />
    public bool IsFinished()
        => _contract?.HasReachedState(UseSkillStates.CharacterRestored) ?? false;

    /// <inheritdoc />
    public Result CanBeUsed(ICombatState combatState)
    {
        if (Skill.Info is null)
        {
            return new MissingInfoError("skill", Skill.SkillVNum);
        }

        var character = combatState.Game.Character;
        if (Target.Hp is not null && Target.Hp.Amount is not null && Target.Hp.Amount == 0)
        {
            return new CannotBeUsedError(CanBeUsedResponse.WontBeUsable, new TargetDeadError());
        }

        if (character is null)
        {
            return new CharacterNotInitializedError();
        }

        if (character.CantAttack)
        {
            return new CannotBeUsedError(CanBeUsedResponse.MustWait, new CharacterCannotAttackError());
        }

        if (character.Mp is not null && character.Mp.Amount is not null)
        {
            if (character.Mp.Amount < Skill.Info.MpCost)
            { // The character is in combat, mp won't restore.
                return new CannotBeUsedError
                (
                    CanBeUsedResponse.WontBeUsable,
                    new NotEnoughManaError(character.Mp.Amount.Value, Skill.Info.MpCost)
                );
            }
        }

        if (Skill.IsOnCooldown)
        {
            return new CannotBeUsedError(CanBeUsedResponse.MustWait, new SkillOnCooldownError(Skill));
        }

        return Result.FromSuccess();
    }

    private Result<IContract<SkillUsedEvent, UseSkillStates>> ContractSkill(ISkillInfo info)
    {
        if (info.AttackType == AttackType.Dash)
        {
            return SkillsApi.ContractUseSkillOn
            (
                Skill,
                info.TargetType == TargetType.Self ? Caster : Target,
                Target.Position!.Value.X,
                Target.Position!.Value.Y
            );
        }

        switch (info.TargetType)
        {
            case TargetType.SelfOrTarget: // a buff?
            case TargetType.Self:
                return SkillsApi.ContractUseSkillOnCharacter(Skill);
            case TargetType.NoTarget: // area skill?
                return SkillsApi.ContractUseSkillAt(Skill, Target.Position!.Value.X, Target.Position.Value.Y);
            case TargetType.Target:
                return SkillsApi.ContractUseSkillOn(Skill, Target);
        }

        throw new UnreachableException();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _contract?.Unregister();
    }
}