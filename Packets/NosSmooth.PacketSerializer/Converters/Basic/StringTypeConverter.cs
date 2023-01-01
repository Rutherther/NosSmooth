//
//  StringTypeConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Remora.Results;

namespace NosSmooth.PacketSerializer.Converters.Basic;

/// <summary>
/// Converter of <see cref="string"/>.
/// </summary>
public class StringTypeConverter : BasicTypeConverter<string>
{
    /// <inheritdoc />
    protected override Result<string?> Deserialize(ReadOnlySpan<char> value)
    {
        return value.ToString();
    }

    /// <inheritdoc />
    protected override string GetNullSymbol()
    {
        return "-";
    }
}