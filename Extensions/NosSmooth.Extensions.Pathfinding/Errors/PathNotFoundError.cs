//
//  PathNotFoundError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Extensions.Pathfinding.Errors;

/// <summary>
/// Could not find path to the given target.
/// </summary>
/// <param name="TargetX">The target x coordinate.</param>
/// <param name="TargetY">The target y coordinate.</param>
public record PathNotFoundError(short TargetX, short TargetY) : ResultError($"Path to {TargetX}, {TargetY} not found.");