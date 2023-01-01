//
//  StringConverterRepository.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.PacketSerializer.Abstractions;
using NosSmooth.PacketSerializer.Converters.Special;
using NosSmooth.PacketSerializer.Errors;
using NosSmooth.PacketSerializer.Extensions;
using Remora.Results;

namespace NosSmooth.PacketSerializer.Converters;

/// <summary>
/// Repository for <see cref="IStringConverter"/>.
/// </summary>
public class StringConverterRepository : IStringConverterRepository
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<Type, IStringConverter?> _typeConverters;
    private IReadOnlyList<IStringConverterFactory>? _converterFactories;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringConverterRepository"/> class.
    /// </summary>
    /// <param name="serviceProvider">The dependency injection service provider.</param>
    public StringConverterRepository(IServiceProvider serviceProvider)
    {
        _typeConverters = new ConcurrentDictionary<Type, IStringConverter?>();
        _converterFactories = null;
        _serviceProvider = serviceProvider;
    }

    private IReadOnlyList<IStringConverterFactory> ConverterFactories
    {
        get
        {
            if (_converterFactories is null)
            {
                _converterFactories = _serviceProvider
                    .GetServices<IStringConverterFactory>()
                    .ToArray();
            }

            return _converterFactories;
        }
    }

    /// <summary>
    /// Gets the type converter for the given type.
    /// </summary>
    /// <param name="type">The type to find converter for.</param>
    /// <returns>The type converter or an error.</returns>
    public Result<IStringConverter> GetTypeConverter(Type type)
    {
        var typeConverter = _typeConverters.GetOrAddResult
        (
            type,
            (getType) =>
            {
                foreach (var converterFactory in ConverterFactories)
                {
                    if (converterFactory.ShouldHandle(getType))
                    {
                        var result = converterFactory.CreateTypeSerializer(getType);
                        if (!result.IsSuccess)
                        {
                            return Result<IStringConverter?>.FromError(result);
                        }

                        return Result<IStringConverter?>.FromSuccess(result.Entity);
                    }
                }

                var converterType = typeof(IStringConverter<>).MakeGenericType(type);
                return Result<IStringConverter?>.FromSuccess
                    ((IStringConverter?)_serviceProvider.GetService(converterType));
            }
        );

        if (!typeConverter.IsSuccess)
        {
            return Result<IStringConverter>.FromError(typeConverter);
        }

        if (typeConverter.Entity is null)
        {
            return new TypeConverterNotFoundError(type);
        }

        return Result<IStringConverter>.FromSuccess(typeConverter.Entity);
    }

    /// <summary>
    /// Gets the type converter for the given type.
    /// </summary>
    /// <typeparam name="TParseType">The type to find converter for.</typeparam>
    /// <returns>The type converter or an error.</returns>
    public Result<IStringConverter<TParseType>> GetTypeConverter<TParseType>()
        => GetTypeConverter(typeof(TParseType)).Cast<IStringConverter<TParseType>, IStringConverter>();
}