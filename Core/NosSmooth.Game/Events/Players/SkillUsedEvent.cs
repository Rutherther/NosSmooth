//
//  SkillUsedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;

namespace NosSmooth.Game.Events.Players;

public record SkillUsedEvent(ILivingEntity? Entity, long EntityId, Skill? Skill, long SkillVNum, long TargetId, Position? TargetPosition) : IGameEvent;