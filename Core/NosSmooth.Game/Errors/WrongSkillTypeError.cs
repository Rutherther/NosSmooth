//
//  WrongSkillTypeError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Game.Apis.Safe;
using Remora.Results;

namespace NosSmooth.Game.Errors;

/// <summary>
/// Used for safe skill api when the skill type does not match the correct one.
/// </summary>
/// <remarks>
/// Use one of the other methods <see cref="o:NostaleSkillsApi.UseSkillOn"/>,
/// <see cref="NostaleSkillsApi.UseSkillAt"/>,  <see cref="NostaleSkillsApi.UseSkillOnCharacter"/>.
/// </remarks>
/// <param name="ExpectedType">The type that was expected.</param>
/// <param name="ActualType">The actual type of the skill.</param>
public record WrongSkillTypeError(SkillType ExpectedType, SkillType ActualType)
    : ResultError($"Cannot use a skill of type {ActualType}, only type {ExpectedType} is allowed.");