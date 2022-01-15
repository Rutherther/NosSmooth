//
//  UShortStringConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.Packets.Errors;
using NosSmooth.PacketSerializer.Abstractions.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Basic;

/// <summary>
/// Converter of <see cref="ushort"/>.
/// </summary>
public class UShortStringConverter : BasicTypeConverter<ushort>
{
    /// <inheritdoc />
    protected override Result<ushort> Deserialize(ReadOnlySpan<char> value)
    {
        if (!ushort.TryParse(value, out var parsed))
        {
            return new CouldNotConvertError(this, value.ToString(), "Could not parse as an ushort.");
        }

        return parsed;
    }
}