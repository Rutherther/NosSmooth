//
//  EntityNotFoundError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Extensions.Combat.Errors;

/// <summary>
/// Matching entity not found error.
/// </summary>
public record EntityNotFoundError() : ResultError("Could not find an entity that matches the conditions.");