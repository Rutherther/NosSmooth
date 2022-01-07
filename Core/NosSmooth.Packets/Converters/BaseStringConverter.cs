//
//  BaseStringConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Converters;

/// <summary>
/// Base type for converting objects that maps object converting methods to the actual type.
/// </summary>
/// <typeparam name="TParseType">The type of the object that this converts.</typeparam>
public abstract class BaseStringConverter<TParseType> : IStringConverter<TParseType>
{
    /// <inheritdoc />
    public abstract Result Serialize(TParseType? obj, PacketStringBuilder builder);

    /// <inheritdoc />
    public abstract Result<TParseType?> Deserialize(PacketStringEnumerator stringEnumerator);

    /// <inheritdoc/>
    Result<object?> IStringConverter.Deserialize(PacketStringEnumerator stringEnumerator)
    {
        var result = Deserialize(stringEnumerator);
        if (!result.IsSuccess)
        {
            return Result<object?>.FromError(result);
        }

        return Result<object?>.FromSuccess(result.Entity);
    }

    /// <inheritdoc/>
    Result IStringConverter.Serialize(object? obj, PacketStringBuilder builder)
    {
        if (!(obj is TParseType parseType))
        {
            return new WrongTypeError(this, typeof(TParseType), obj);
        }

        return Serialize(parseType, builder);
    }
}