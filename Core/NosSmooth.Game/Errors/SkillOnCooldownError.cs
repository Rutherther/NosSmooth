//
//  SkillOnCooldownError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Characters;
using Remora.Results;

namespace NosSmooth.Game.Errors;

/// <summary>
/// Acts as an error specifying the skill is on cooldown.
/// </summary>
public record SkillOnCooldownError(Skill skill) : ResultError("The skill is on cooldown.");