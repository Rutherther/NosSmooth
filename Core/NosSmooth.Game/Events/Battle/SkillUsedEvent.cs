//
//  SkillUsedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;
using NosSmooth.Packets.Enums.Battle;

namespace NosSmooth.Game.Events.Battle;

/// <summary>
/// A skill has been used.
/// </summary>
/// <param name="Caster">The caster entity of the skill.</param>
/// <param name="Target">The target entity of the skill.</param>
/// <param name="Skill">The skill that has been used with the information about the skill.</param>
/// <param name="SkillVNum">The vnum of the skill.</param>
/// <param name="TargetPosition">The position of the target.</param>
/// <param name="Hit"></param>
/// <param name="Damage"></param>
public record SkillUsedEvent
(
    ILivingEntity Caster,
    ILivingEntity Target,
    Skill Skill,
    Position? TargetPosition,
    HitMode? Hit,
    uint Damage
) : IGameEvent;