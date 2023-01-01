//
//  UseSkillOperation.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Xml.XPath;
using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Extensions.Combat.Errors;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Packets;
using NosSmooth.Packets.Client.Battle;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Operations;

/// <summary>
/// A combat operation to use a skill.
/// </summary>
/// <param name="Skill">The skill to use.</param>
/// <param name="Caster">The caster entity that is using the skill.</param>
/// <param name="Target">The target entity to use the skill at.</param>
public record UseSkillOperation(Skill Skill, ILivingEntity Caster, ILivingEntity Target) : ICombatOperation
{
    /// <inheritdoc />
    public Result<CanBeUsedResponse> CanBeUsed(ICombatState combatState)
    {
        if (Skill.Info is null)
        {
            return new MissingInfoError("skill", Skill.SkillVNum);
        }

        var character = combatState.Game.Character;
        if (character is not null && character.Mp is not null && character.Mp.Amount is not null)
        {
            if (character.Mp.Amount < Skill.Info.MpCost)
            { // The character is in combat, mp won't restore.
                return CanBeUsedResponse.WontBeUsable;
            }
        }

        return Skill.IsOnCooldown ? CanBeUsedResponse.MustWait : CanBeUsedResponse.CanBeUsed;
    }

    /// <inheritdoc />
    public async Task<Result> UseAsync(ICombatState combatState, CancellationToken ct = default)
    {
        if (Skill.Info is null)
        {
            return new MissingInfoError("skill", Skill.SkillVNum);
        }

        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(ct);
        await combatState.CombatManager.RegisterSkillCancellationTokenAsync(linkedSource, ct);
        var sendResponse = await combatState.Client.SendPacketAsync
        (
            CreateSkillUsePacket(Skill.Info),
            ct
        );

        if (!sendResponse.IsSuccess)
        {
            await combatState.CombatManager.UnregisterSkillCancellationTokenAsync(linkedSource, ct);
            return sendResponse;
        }

        try
        {
            // wait 10 times the cast delay in case su is not received.
            await Task.Delay(Skill.Info.CastTime * 1000, linkedSource.Token);
        }
        catch (TaskCanceledException)
        {
            // ignored
        }
        await combatState.CombatManager.UnregisterSkillCancellationTokenAsync(linkedSource, ct);
        await Task.Delay(1000, ct);

        return Result.FromSuccess();
    }

    private IPacket CreateSkillUsePacket(ISkillInfo info)
    {
        switch (info.TargetType)
        {
            case TargetType.SelfOrTarget: // a buff?
            case TargetType.Self:
                return CreateSelfTargetedSkillPacket(info);
            case TargetType.NoTarget: // area skill?
                return CreateAreaSkillPacket(info);
            case TargetType.Target:
                return CreateTargetedSkillPacket(info);
        }

        throw new UnreachableException();
    }

    private IPacket CreateAreaSkillPacket(ISkillInfo info)
        => new UseAOESkillPacket
        (
            info.CastId,
            Target.Position!.Value.X,
            Target.Position.Value.Y
        );

    private IPacket CreateTargetedSkillPacket(ISkillInfo info)
        => new UseSkillPacket
        (
            info.CastId,
            Target.Type,
            Target.Id,
            null,
            null
        );

    private IPacket CreateSelfTargetedSkillPacket(ISkillInfo info)
        => new UseSkillPacket
        (
            info.CastId,
            Caster.Type,
            Caster.Id,
            null,
            null
        );
}