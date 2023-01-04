//
//  AoESkillUsedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;

namespace NosSmooth.Game.Events.Battle;

/// <summary>
/// An AoE skill has been used. (bs packet)
/// </summary>
/// <remarks>
/// The damage to various entities will be sent in respective Su packets.
/// TODO find out connections between su and bs packets.
/// </remarks>
public record AoESkillUsedEvent
(
    ILivingEntity Caster,
    Skill Skill,
    Position TargetPosition
) : IGameEvent;