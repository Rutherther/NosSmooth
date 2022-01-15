//
//  BindingNotFoundError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.LocalBinding.Errors;

/// <summary>
/// The memory pattern was not found in the memory.
/// </summary>
/// <param name="Pattern">The pattern that could not be found.</param>
/// <param name="Path">The entity the pattern should represent.</param>
public record BindingNotFoundError(string Pattern, string Path)
    : ResultError($"Could not find pattern ({Pattern}) in the memory while searching for {Path}.");
