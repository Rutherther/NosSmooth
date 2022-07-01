//
//  SkillNotFoundError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Extensions.Combat.Errors;

/// <summary>
/// Matchin skill not found.
/// </summary>
public record SkillNotFoundError() : ResultError("Could not find a skill that matches the conditions.");