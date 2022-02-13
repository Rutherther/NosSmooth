//
//  StateNotInitializedError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Extensions.Pathfinding.Errors;

/// <summary>
/// Pathfinder state not initialized.
/// </summary>
public record StateNotInitializedError() : ResultError
    ("The pathfinder state is not yet initialized, the map is unknown. Must wait for c_map packet.");