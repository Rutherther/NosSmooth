//
//  ItemNotFoundError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Extensions.Combat.Errors;

/// <summary>
/// Matchin item not found error.
/// </summary>
public record ItemNotFoundError() : ResultError("Could not find an item that matches the conditions.");