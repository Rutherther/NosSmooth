//
//  CouldNotInitializeModuleError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.LocalBinding.Errors;

/// <summary>
/// Could not initialize the given NosTale module.
/// </summary>
/// <param name="Module">The module type that could not be initialized.</param>
/// <param name="UnderlyingError">The error why the module could not be initialized.</param>
public record CouldNotInitializeModuleError
    (Type Module, IResultError UnderlyingError) : ResultError($"Could initialize a nostale module {Module.FullName}.");