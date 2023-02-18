//
//  BoolStringConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.PacketSerializer.Abstractions;
using Remora.Results;

namespace NosSmooth.PacketSerializer.Converters.Basic;

/// <summary>
/// Converter of <see cref="bool"/>.
/// </summary>
public class BoolStringConverter : BasicTypeConverter<bool>
{
    /// <inheritdoc />
    public override Result Serialize(bool obj, ref PacketStringBuilder builder)
    {
        builder.Append(obj ? '1' : '0');
        return Result.FromSuccess();
    }

    /// <inheritdoc />
    protected override Result<bool> Deserialize(ReadOnlySpan<char> value)
    {
        return value[0] == '1';
    }
}