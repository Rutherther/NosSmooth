//
//  StringSerializer.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Special;

/// <inheritdoc />
public class StringSerializer : IStringSerializer
{
    private readonly IStringConverterRepository _converterRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringSerializer"/> class.
    /// </summary>
    /// <param name="converterRepository">The repository of string converters.</param>
    public StringSerializer(IStringConverterRepository converterRepository)
    {
        _converterRepository = converterRepository;
    }

    /// <inheritdoc />
    public Result<object?> Deserialize(Type parseType, PacketStringEnumerator stringEnumerator)
    {
        var converterResult = _converterRepository.GetTypeConverter(parseType);
        if (!converterResult.IsSuccess)
        {
            return Result<object?>.FromError(converterResult);
        }

        var deserializedResult = converterResult.Entity.Deserialize(stringEnumerator);
        if (!deserializedResult.IsSuccess)
        {
            return Result<object?>.FromError(deserializedResult);
        }

        return Result<object?>.FromSuccess(deserializedResult.Entity);
    }

    /// <inheritdoc />
    public Result Serialize(Type parseType, object? obj, PacketStringBuilder builder)
    {
        var converterResult = _converterRepository.GetTypeConverter(parseType);
        if (!converterResult.IsSuccess)
        {
            return Result.FromError(converterResult);
        }

        return converterResult.Entity.Serialize(obj, builder);
    }

    /// <inheritdoc />
    public Result<TParseType?> Deserialize<TParseType>(PacketStringEnumerator stringEnumerator)
    {
        var converterResult = _converterRepository.GetTypeConverter<TParseType>();
        if (!converterResult.IsSuccess)
        {
            return Result<TParseType?>.FromError(converterResult);
        }

        return converterResult.Entity.Deserialize(stringEnumerator);
    }

    /// <inheritdoc />
    public Result Serialize<TParseType>(TParseType? obj, PacketStringBuilder builder)
    {
        var converterResult = _converterRepository.GetTypeConverter<TParseType>();
        if (!converterResult.IsSuccess)
        {
            return Result.FromError(converterResult);
        }

        return converterResult.Entity.Serialize(obj, builder);
    }
}