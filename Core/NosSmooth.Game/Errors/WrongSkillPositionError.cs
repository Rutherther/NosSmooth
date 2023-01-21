//
//  WrongSkillPositionError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Enums;
using Remora.Results;

namespace NosSmooth.Game.Errors;

/// <summary>
/// Dash skills have to have a position specified,
/// non-dash skills cannot have position specified.
/// In case this is not respected in the skills api,
/// this error will be returned.
/// </summary>
/// <param name="AttackType">The type of the skill.</param>
public record WrongSkillPositionError(AttackType AttackType)
    : ResultError($"The skill with an attack type {AttackType} has to have a map position specified.");