//
//  WrongSkillTypeError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Enums;
using Remora.Results;

namespace NosSmooth.Game.Errors;

public record WrongSkillTypeError(SkillType ExpectedType, SkillType ActualType)
    : ResultError($"Cannot use a skill of type {ActualType}, only type {ExpectedType} is allowed.");