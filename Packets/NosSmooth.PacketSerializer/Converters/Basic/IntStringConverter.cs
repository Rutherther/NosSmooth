//
//  IntStringConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.PacketSerializer.Abstractions.Errors;
using Remora.Results;

namespace NosSmooth.PacketSerializer.Converters.Basic;

/// <summary>
/// Converter of <see cref="int"/>.
/// </summary>
public class IntStringConverter : SpanFormattableTypeConverter<int>
{
    /// <inheritdoc />
    protected override Result<int> Deserialize(ReadOnlySpan<char> value)
    {
        if (!int.TryParse(value, out var parsed))
        {
            return new CouldNotConvertError(this, value.ToString(), "Could not parse as int.");
        }

        return parsed;
    }
}