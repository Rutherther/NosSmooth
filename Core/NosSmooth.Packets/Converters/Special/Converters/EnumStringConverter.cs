//
//  EnumStringConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Special.Converters;

/// <summary>
/// Converts enum with the given underlying type.
/// </summary>
/// <typeparam name="TEnum">The enum.</typeparam>
/// <typeparam name="TUnderlyingType">The enum's underlying type.</typeparam>
public class EnumStringConverter<TEnum, TUnderlyingType> : BaseStringConverter<TEnum>
{
    private readonly IStringSerializer _serializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumStringConverter{TEnum, TUnderlyingType}"/> class.
    /// </summary>
    /// <param name="serializer">The string serializer.</param>
    public EnumStringConverter(IStringSerializer serializer)
    {
        _serializer = serializer;
    }

    /// <inheritdoc />
    public override Result Serialize(TEnum? obj, PacketStringBuilder builder)
    {
        builder.Append(((TUnderlyingType?)(object?)obj)?.ToString() ?? "-");
        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public override Result<TEnum?> Deserialize(ref PacketStringEnumerator stringEnumerator)
    {
        var result = _serializer.Deserialize<TUnderlyingType>(ref stringEnumerator);
        if (!result.IsSuccess)
        {
            return Result<TEnum?>.FromError(result);
        }

        return (TEnum?)(object?)result.Entity;
    }
}