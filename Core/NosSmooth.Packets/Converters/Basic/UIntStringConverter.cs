//
//  UIntStringConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Basic;

/// <summary>
/// Converter of <see cref="uint"/>.
/// </summary>
public class UIntStringConverter : BasicTypeConverter<uint>
{
    /// <inheritdoc />
    protected override Result<uint> Deserialize(ReadOnlySpan<char> value)
    {
        if (!uint.TryParse(value, out var parsed))
        {
            return new CouldNotConvertError(this, value.ToString(), "Could not parse as uint");
        }

        return parsed;
    }
}