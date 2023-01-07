//
//  NullableWrapperConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.PacketSerializer.Abstractions;
using Remora.Results;

namespace NosSmooth.PacketSerializer.Converters.Common;

/// <summary>
/// Converter of <see cref="NullableWrapper{T}"/>.
/// </summary>
/// <typeparam name="T">The underlying type.</typeparam>
public class NullableWrapperConverter<T> : BaseStringConverter<NullableWrapper<T>>
{
    private readonly IStringConverterRepository _converterRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableWrapperConverter{T}"/> class.
    /// </summary>
    /// <param name="converterRepository">The converter repository.</param>
    public NullableWrapperConverter(IStringConverterRepository converterRepository)
    {
        _converterRepository = converterRepository;
    }

    /// <inheritdoc />
    public override Result Serialize(NullableWrapper<T> obj, PacketStringBuilder builder)
    {
        if (obj.Value is null)
        {
            builder.Append("-1");
        }
        else
        {
            var converterResult = _converterRepository.GetTypeConverter<T>();
            if (!converterResult.IsDefined(out var converter))
            {
                return Result.FromError(converterResult);
            }

            return converter.Serialize(obj.Value, builder);
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public override Result<NullableWrapper<T>> Deserialize(ref PacketStringEnumerator stringEnumerator, DeserializeOptions options)
    {
        var tokenResult = stringEnumerator.GetNextToken(out var packetToken, false);
        if (!tokenResult.IsSuccess)
        {
            return Result<NullableWrapper<T>>.FromError(tokenResult);
        }

        if (packetToken.Token.Length == 2 && packetToken.Token.StartsWith("-1"))
        {
            return Result<NullableWrapper<T>>.FromSuccess(new NullableWrapper<T>(default));
        }

        var converterResult = _converterRepository.GetTypeConverter<T>();
        if (!converterResult.IsDefined(out var converter))
        {
            return Result<NullableWrapper<T>>.FromError(converterResult);
        }

        var deserializationResult = converter.Deserialize(ref stringEnumerator, new DeserializeOptions(true));
        if (!deserializationResult.IsDefined(out var deserialization))
        {
            return Result<NullableWrapper<T>>.FromError(deserializationResult);
        }

        return new NullableWrapper<T>(deserialization);
    }
}