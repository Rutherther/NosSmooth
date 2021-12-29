//
//  CouldNotConvertError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.Packets.Converters;
using Remora.Results;

namespace NosSmooth.Packets.Errors;

/// <summary>
/// The value could not be converted.
/// </summary>
/// <param name="Converter">The converter that failed the parsing.</param>
/// <param name="Value">The value that failed to parse.</param>
/// <param name="Reason">The reason for the error.</param>
/// <param name="Exception">The underlying exception, if any.</param>
public record CouldNotConvertError(ITypeConverter Converter, string Value, string Reason, Exception? Exception = default)
    : ResultError($"Converter {Converter.GetType().FullName} could not convert {Value} due to {Reason}.");