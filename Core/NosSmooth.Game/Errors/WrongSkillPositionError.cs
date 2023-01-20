//
//  WrongSkillPositionError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Enums;
using Remora.Results;

namespace NosSmooth.Game.Errors;

public record WrongSkillPositionError(AttackType AttackType)
    : ResultError($"The skill with an attack type {AttackType} has to have a map position specified.");