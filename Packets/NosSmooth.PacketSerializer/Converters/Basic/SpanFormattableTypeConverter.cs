//
//  SpanFormattableTypeConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.PacketSerializer.Abstractions;
using Remora.Results;

namespace NosSmooth.PacketSerializer.Converters.Basic;

/// <summary>
/// A converter for <see cref="ISpanFormattable"/> types such as int, long etc..
/// </summary>
/// <typeparam name="T">The span formattable type.</typeparam>
public abstract class SpanFormattableTypeConverter<T> : BasicTypeConverter<T>
    where T : ISpanFormattable
{
    /// <inheritdoc />
    public override Result Serialize(T? obj, in PacketStringBuilder builder)
    {
        if (obj is not null)
        {
            builder.Append(obj);
        }
        else
        {
            builder.Append(GetNullSymbol());
        }

        return Result.FromSuccess();
    }
}