//
//  ULongStringConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.Packets.Errors;
using NosSmooth.PacketSerializer.Abstractions.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Basic;

/// <summary>
/// Converter of <see cref="ulong"/>.
/// </summary>
public class ULongStringConverter : BasicTypeConverter<ulong>
{
    /// <inheritdoc />
    protected override Result<ulong> Deserialize(ReadOnlySpan<char> value)
    {
        if (!ulong.TryParse(value, out var parsed))
        {
            return new CouldNotConvertError(this, value.ToString(), "Could not parse as an ulong.");
        }

        return parsed;
    }
}