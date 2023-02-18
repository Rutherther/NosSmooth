//
//  OptionalWrapperConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.PacketSerializer.Abstractions;
using Remora.Results;

namespace NosSmooth.PacketSerializer.Converters.Common;

/// <summary>
/// Converter of <see cref="OptionalWrapper{T}"/>.
/// </summary>
/// <typeparam name="T">The underlying type.</typeparam>
public class OptionalWrapperConverter<T> : BaseStringConverter<OptionalWrapper<T>>
{
    private readonly IStringConverterRepository _converterRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionalWrapperConverter{T}"/> class.
    /// </summary>
    /// <param name="converterRepository">The converter repository.</param>
    public OptionalWrapperConverter(IStringConverterRepository converterRepository)
    {
        _converterRepository = converterRepository;
    }

    /// <inheritdoc />
    public override Result Serialize(OptionalWrapper<T> obj, ref PacketStringBuilder builder)
    {
        if (obj.Value is null)
        {
            return Result.FromSuccess();
        }

        var converterResult = _converterRepository.GetTypeConverter<T>();
        if (!converterResult.IsDefined(out var converter))
        {
            return Result.FromError(converterResult);
        }

        return converter.Serialize(obj.Value, ref builder);

    }

    /// <inheritdoc />
    public override Result<OptionalWrapper<T>> Deserialize(ref PacketStringEnumerator stringEnumerator, DeserializeOptions options)
    {
        if (stringEnumerator.IsOnLastToken() ?? false)
        {
            return Result<OptionalWrapper<T>>.FromSuccess(new OptionalWrapper<T>(false, default));
        }

        var tokenResult = stringEnumerator.GetNextToken(out var token, false);
        if (!tokenResult.IsSuccess)
        {
            return Result<OptionalWrapper<T>>.FromError(tokenResult);
        }

        if (token.Token.Length == 0)
        {
            stringEnumerator.GetNextToken(out _); // seek
            return Result<OptionalWrapper<T>>.FromSuccess(new OptionalWrapper<T>(false, default));
        }

        var converterResult = _converterRepository.GetTypeConverter<T>();
        if (!converterResult.IsDefined(out var converter))
        {
            return Result<OptionalWrapper<T>>.FromError(converterResult);
        }

        var deserializationResult = converter.Deserialize(ref stringEnumerator, new DeserializeOptions(true));
        if (!deserializationResult.IsDefined(out var deserialization))
        {
            return Result<OptionalWrapper<T>>.FromError(deserializationResult);
        }

        return new OptionalWrapper<T>(true, deserialization);
    }
}