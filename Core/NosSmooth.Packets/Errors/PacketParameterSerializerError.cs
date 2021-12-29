//
//  PacketParameterSerializerError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.Packets.Converters;
using Remora.Results;

namespace NosSmooth.Packets.Errors;

/// <summary>
/// Could not deserialize one of the packet's properties.
/// </summary>
/// <param name="Converter">The converter that could not deserialize the parameter.</param>
/// <param name="PropertyName">The name of the property.</param>
/// <param name="Result">The underlying result.</param>
/// <param name="Reason">The reason for the error, if known.</param>
public record PacketParameterSerializerError(ITypeConverter Converter, string PropertyName, IResult Result, string? Reason = null)
    : ResultError($"There was an error deserializing property {PropertyName} in converter {Converter.GetType().FullName}{(Reason is not null ? (", reason: " + Reason) : string.Empty)}");