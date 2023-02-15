//
//  EntityStateNotFoundError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Extensions.Pathfinding.Errors;

public record EntityStateNotFoundError(long EntityId) : ResultError($"Could not find pathfinder state of entity {EntityId}");