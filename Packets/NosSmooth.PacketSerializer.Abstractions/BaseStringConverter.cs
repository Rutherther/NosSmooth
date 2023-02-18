//
//  BaseStringConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Errors;
using Remora.Results;

namespace NosSmooth.PacketSerializer.Abstractions;

/// <summary>
/// Base type for converting objects that maps object converting methods to the actual type.
/// </summary>
/// <typeparam name="TParseType">The type of the object that this converts.</typeparam>
public abstract class BaseStringConverter<TParseType> : IStringConverter<TParseType>
{
    /// <inheritdoc />
    public abstract Result Serialize(TParseType? obj, ref PacketStringBuilder builder);

    /// <inheritdoc />
    public abstract Result<TParseType?> Deserialize(ref PacketStringEnumerator stringEnumerator, DeserializeOptions options);

    /// <inheritdoc/>
    Result<object?> IStringConverter.Deserialize(ref PacketStringEnumerator stringEnumerator, DeserializeOptions options)
    {
        var result = Deserialize(ref stringEnumerator, options);
        if (!result.IsSuccess)
        {
            return Result<object?>.FromError(result);
        }

        return Result<object?>.FromSuccess(result.Entity);
    }

    /// <inheritdoc/>
    Result IStringConverter.Serialize(object? obj, ref PacketStringBuilder builder)
    {
        if (!(obj is TParseType parseType))
        {
            return new WrongTypeError(this, typeof(TParseType), obj);
        }

        return Serialize(parseType, ref builder);
    }
}
