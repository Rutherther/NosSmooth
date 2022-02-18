//
//  MapNotInitializedError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Extensions.Combat.Errors;

/// <summary>
/// The map is not initialized.
/// </summary>
public record MapNotInitializedError()
    : ResultError("The map is not yet initialized.");