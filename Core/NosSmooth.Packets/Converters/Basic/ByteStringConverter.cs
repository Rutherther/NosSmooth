//
//  ByteStringConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Basic;

/// <summary>
/// Converter of <see cref="byte"/>.
/// </summary>
public class ByteStringConverter : BasicTypeConverter<byte>
{
    /// <inheritdoc />
    protected override Result<byte> Deserialize(ReadOnlySpan<char> value)
    {
        if (!byte.TryParse(value, out var parsed))
        {
            return new CouldNotConvertError(this, value.ToString(), "Could not parse as an byte.");
        }

        return parsed;
    }
}