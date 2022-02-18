//
//  UseSkillOperation.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Xml.XPath;
using NosSmooth.Extensions.Combat.Errors;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Packets.Client.Battle;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Operations;

/// <summary>
/// A combat operation to use a skill.
/// </summary>
/// <param name="Skill">The skill to use.</param>
/// <param name="Target">The target entity to use the skill at.</param>
public record UseSkillOperation(Skill Skill, ILivingEntity Target) : ICombatOperation
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

        // TODO: support for area skills, support skills that use x, y coordinates (like dashes or teleports)
        var sendResponse = await combatState.Client.SendPacketAsync
        (
            new UseSkillPacket
            (
                Skill.Info.CastId,
                Target.Type,
                Target.Id,
                null,
                null
            ),
            ct
        );

        if (!sendResponse.IsSuccess)
        {
            return sendResponse;
        }

        var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(ct);
        combatState.CombatManager.RegisterSkillCancellationToken(linkedSource);
        await Task.Delay(Skill.Info.CastTime * 200 * 5, linkedSource.Token);
        combatState.CombatManager.UnregisterSkillCancellationToken(linkedSource);

        return Result.FromSuccess();
    }
}