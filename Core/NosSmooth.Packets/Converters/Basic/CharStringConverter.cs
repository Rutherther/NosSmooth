//
//  CharStringConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Basic;

/// <summary>
/// Converter of <see cref="char"/>.
/// </summary>
public class CharStringConverter : BasicTypeConverter<char>
{
    /// <inheritdoc />
    protected override Result<char> Deserialize(ReadOnlySpan<char> value)
    {
        if (value.Length != 1)
        {
            return new CouldNotConvertError(this, value.ToString(), "The token is not one character long.");
        }

        return value[0];
    }
}