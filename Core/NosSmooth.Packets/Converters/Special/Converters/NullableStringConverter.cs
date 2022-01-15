//
//  NullableStringConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Special.Converters;

#pragma warning disable SA1125
/// <summary>
/// Converter of nullable types.
/// </summary>
/// <typeparam name="T">The nonnullable underlying type.</typeparam>
public class NullableStringConverter<T> : BaseStringConverter<Nullable<T>>
    where T : struct
{
    private readonly IStringSerializer _stringSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableStringConverter{T}"/> class.
    /// </summary>
    /// <param name="stringSerializer">The string serializer.</param>
    public NullableStringConverter(IStringSerializer stringSerializer)
    {
        _stringSerializer = stringSerializer;

    }

    /// <inheritdoc />
    public override Result Serialize(T? obj, PacketStringBuilder builder)
    {
        if (obj is null)
        {
            builder.Append('-');
            return Result.FromSuccess();
        }

        return _stringSerializer.Serialize<T>(obj.Value, builder);
    }

    /// <inheritdoc />
    public override Result<T?> Deserialize(ref PacketStringEnumerator stringEnumerator)
    {
        var result = _stringSerializer.Deserialize<T>(ref stringEnumerator);
        if (!result.IsSuccess)
        {
            return Result<T?>.FromError(result);
        }

        return Result<T?>.FromSuccess(result.Entity);
    }
}
#pragma warning restore SA1125
