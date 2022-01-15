//
//  CouldNotConvertError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using NosSmooth.Packets.Converters;
using Remora.Results;

namespace NosSmooth.Packets.Errors;

/// <summary>
/// The value could not be converted.
/// </summary>
/// <param name="Converter">The converter that failed the parsing.</param>
/// <param name="Value">The value that failed to parse.</param>
/// <param name="Reason">The reason for the error.</param>
public record CouldNotConvertError(IStringConverter Converter, string Value, string Reason)
    : ResultError($"Converter {Converter.GetType().FullName} could not convert value \"{Value}\" due to {Reason}.");