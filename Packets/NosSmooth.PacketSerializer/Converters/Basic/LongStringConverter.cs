//
//  LongStringConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.PacketSerializer.Abstractions.Errors;
using Remora.Results;

namespace NosSmooth.PacketSerializer.Converters.Basic;

/// <summary>
/// Converter of <see cref="long"/>.
/// </summary>
public class LongStringConverter : BasicTypeConverter<long>
{
    /// <inheritdoc />
    protected override Result<long> Deserialize(ReadOnlySpan<char> value)
    {
        if (!long.TryParse(value, out var parsed))
        {
            return new CouldNotConvertError(this, value.ToString(), "Could not parse as a long.");
        }

        return parsed;
    }
}