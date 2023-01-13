//
//  WrongSkillTargetError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Entities;
using Remora.Results;

namespace NosSmooth.Game.Errors;

/// <summary>
/// Skill was used at wrong target.
/// </summary>
/// <param name="Skill">The skill that was used.</param>
/// <param name="Target">The target of the skill.</param>
public record WrongSkillTargetError(Skill Skill, ILivingEntity? Target)
    : ResultError($"The skill {Skill.SkillVNum} was used at wrong target {Target}");