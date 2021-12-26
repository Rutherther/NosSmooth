//
//  SkillReadyEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Characters;

namespace NosSmooth.Game.Events.Characters;

/// <summary>
/// The skill cooldown has been up.
/// </summary>
public record SkillReadyEvent(Skill? Skill, long SkillVNum) : IGameEvent;