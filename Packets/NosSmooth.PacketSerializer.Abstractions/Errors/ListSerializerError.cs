//
//  ListSerializerError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.PacketSerializer.Abstractions.Errors;

/// <summary>
/// Could not parse an item in the list.
/// </summary>
/// <param name="Result">The errorful result.</param>
/// <param name="Index">The index in the list.</param>
public record ListSerializerError(IResult Result, int Index) : ResultError($"Could not parse an item (index {Index}) from the list. {Result.Error!.Message}");