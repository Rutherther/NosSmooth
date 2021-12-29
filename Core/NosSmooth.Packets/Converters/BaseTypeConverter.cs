﻿//
//  BaseTypeConverter.cs
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
public abstract class BaseTypeConverter<TParseType> : ITypeConverter<TParseType>
{
    /// <inheritdoc />
    public abstract Result Serialize(TParseType? obj, PacketStringBuilder builder);

    /// <inheritdoc />
    public abstract Result<TParseType?> Deserialize(PacketStringEnumerator stringEnumerator);

    /// <inheritdoc/>
    Result<object?> ITypeConverter.Deserialize(PacketStringEnumerator stringEnumerator)
        => Deserialize(stringEnumerator);

    /// <inheritdoc/>
    Result ITypeConverter.Serialize(object? obj, PacketStringBuilder builder)
    {
        if (!(obj is TParseType parseType))
        {
            return new WrongTypeError(this, typeof(TParseType), obj);
        }

        return Serialize(parseType, builder);
    }
}