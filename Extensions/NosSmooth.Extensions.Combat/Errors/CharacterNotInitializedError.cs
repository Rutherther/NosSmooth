//
//  CharacterNotInitializedError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Extensions.Combat.Errors;

/// <summary>
/// The character is not initialized.
/// </summary>
public record CharacterNotInitializedError(string Field = "")
    : ResultError($"The character {Field} is not yet initialized.");