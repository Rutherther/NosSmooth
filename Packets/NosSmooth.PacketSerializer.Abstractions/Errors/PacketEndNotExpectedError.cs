//
//  PacketEndNotExpectedError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.PacketSerializer.Abstractions.Errors;

/// <summary>
/// The end of a packet was not expected.
/// </summary>
/// <param name="Converter">The type converter.</param>
/// <param name="PropertyName">The property name.</param>
public record PacketEndNotExpectedError(IStringConverter Converter, string PropertyName)
    : ResultError($"Unexpected packet end reached in {Converter.GetType()} during deserializing the property {PropertyName}");