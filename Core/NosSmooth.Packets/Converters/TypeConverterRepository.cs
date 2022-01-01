//
//  TypeConverterRepository.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Packets.Converters.Special;
using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Converters;

/// <summary>
/// Repository for <see cref="ITypeConverter"/>.
/// </summary>
public class TypeConverterRepository : ITypeConverterRepository
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<Type, ITypeConverter?> _typeConverters;
    private IReadOnlyList<ISpecialTypeConverter>? _specialTypeConverters;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeConverterRepository"/> class.
    /// </summary>
    /// <param name="serviceProvider">The dependency injection service provider.</param>
    public TypeConverterRepository(IServiceProvider serviceProvider)
    {
        _typeConverters = new ConcurrentDictionary<Type, ITypeConverter?>();
        _specialTypeConverters = null;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Gets the type converter for the given type.
    /// </summary>
    /// <param name="type">The type to find converter for.</param>
    /// <returns>The type converter or an error.</returns>
    public Result<ITypeConverter> GetTypeConverter(Type type)
    {
        var typeConverter = _typeConverters.GetOrAdd(type, (getType) =>
        {
            var converterType = typeof(ITypeConverter<>).MakeGenericType(type);
            return (ITypeConverter?)_serviceProvider.GetService(converterType);
        });

        if (typeConverter is null)
        {
            return new TypeConverterNotFoundError(type);
        }

        return Result<ITypeConverter>.FromSuccess(typeConverter);
    }

    /// <summary>
    /// Gets the type converter for the given type.
    /// </summary>
    /// <typeparam name="TParseType">The type to find converter for.</typeparam>
    /// <returns>The type converter or an error.</returns>
    public Result<ITypeConverter<TParseType>> GetTypeConverter<TParseType>()
    {
        var typeConverter = _typeConverters.GetOrAdd(
            typeof(TParseType),
            _ => _serviceProvider.GetService<ITypeConverter<TParseType>>()
        );

        if (typeConverter is null)
        {
            return new TypeConverterNotFoundError(typeof(TParseType));
        }

        return Result<ITypeConverter<TParseType>>.FromSuccess((ITypeConverter<TParseType>)typeConverter);
    }

    /// <summary>
    /// Convert the data from the enumerator to the given type.
    /// </summary>
    /// <param name="parseType">The type of the object to serialize.</param>
    /// <param name="stringEnumerator">The packet string enumerator with the current position.</param>
    /// <returns>The parsed object or an error.</returns>
    public Result<object?> Deserialize(Type parseType, PacketStringEnumerator stringEnumerator)
    {
        var specialConverter = GetSpecialConverter(parseType);
        if (specialConverter is not null)
        {
            var deserializeResult = specialConverter.Deserialize(parseType, stringEnumerator);
            if (!deserializeResult.IsSuccess)
            {
                return Result<object?>.FromError(deserializeResult);
            }

            if (deserializeResult.Entity is null)
            {
                if (parseType.DeclaringType == typeof(Nullable<>))
                {
                    return default;
                }

                return Result<object?>.FromError(new DeserializedValueNullError(parseType));
            }

            return deserializeResult.Entity;
        }

        var converterResult = GetTypeConverter(parseType);
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

    /// <summary>
    /// Serializes the given object to string by appending to the packet string builder.
    /// </summary>
    /// <param name="parseType">The type of the object to serialize.</param>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="builder">The string builder to append to.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result Serialize(Type parseType, object? obj, PacketStringBuilder builder)
    {
        var specialConverter = GetSpecialConverter(parseType);
        if (specialConverter is not null)
        {
            return specialConverter.Serialize(parseType, obj, builder);
        }

        var converterResult = GetTypeConverter(parseType);
        if (!converterResult.IsSuccess)
        {
            return Result.FromError(converterResult);
        }

        return converterResult.Entity.Serialize(obj, builder);
    }

    /// <summary>
    /// Convert the data from the enumerator to the given type.
    /// </summary>
    /// <param name="stringEnumerator">The packet string enumerator with the current position.</param>
    /// <typeparam name="TParseType">The type of the object to serialize.</typeparam>
    /// <returns>The parsed object or an error.</returns>
    public Result<TParseType?> Deserialize<TParseType>(PacketStringEnumerator stringEnumerator)
    {
        var specialConverter = GetSpecialConverter(typeof(TParseType));
        if (specialConverter is not null)
        {
            var deserializeResult = specialConverter.Deserialize(typeof(TParseType), stringEnumerator);
            if (!deserializeResult.IsSuccess)
            {
                return Result<TParseType?>.FromError(deserializeResult);
            }

            if (deserializeResult.Entity is null)
            {
                if (typeof(TParseType).DeclaringType == typeof(Nullable<>))
                {
                    return default;
                }

                return Result<TParseType?>.FromError(new DeserializedValueNullError(typeof(TParseType)));
            }

            return (TParseType?)deserializeResult.Entity;
        }

        var converterResult = GetTypeConverter<TParseType>();
        if (!converterResult.IsSuccess)
        {
            return Result<TParseType?>.FromError(converterResult);
        }

        return converterResult.Entity.Deserialize(stringEnumerator);
    }

    /// <summary>
    /// Serializes the given object to string by appending to the packet string builder.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="builder">The string builder to append to.</param>
    /// <typeparam name="TParseType">The type of the object to deserialize.</typeparam>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result Serialize<TParseType>(TParseType? obj, PacketStringBuilder builder)
    {
        if (obj is null)
        {
            builder.Append("-");
            return Result.FromSuccess();
        }

        var specialConverter = GetSpecialConverter(typeof(TParseType));
        if (specialConverter is not null)
        {
            return specialConverter.Serialize(typeof(TParseType), obj, builder);
        }

        var converterResult = GetTypeConverter<TParseType>();
        if (!converterResult.IsSuccess)
        {
            return Result.FromError(converterResult);
        }

        return converterResult.Entity.Serialize(obj, builder);
    }

    private ISpecialTypeConverter? GetSpecialConverter(Type type)
    {
        if (_specialTypeConverters is null)
        {
            _specialTypeConverters = _serviceProvider.GetServices<ISpecialTypeConverter>().ToList();
        }

        foreach (var specialConverter in _specialTypeConverters)
        {
            if (specialConverter.ShouldHandle(type))
            {
                return specialConverter;
            }
        }

        return null;
    }
}