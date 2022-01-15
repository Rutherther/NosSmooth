//
//  DeserializedValueNullError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.PacketSerializer.Abstractions.Errors;

/// <summary>
/// Deserialized value is null, but it cannot be.
/// </summary>
public record DeserializedValueNullError(Type ParseType) : ResultError($"Got a value of type {ParseType.FullName} as null even though it's non-nullable");