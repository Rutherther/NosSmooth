//
//  EnumStringConverterFactory.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.PacketSerializer.Abstractions;
using NosSmooth.PacketSerializer.Converters.Special.Converters;
using Remora.Results;

namespace NosSmooth.PacketSerializer.Converters.Special;

/// <summary>
/// Factory for all enum converters.
/// </summary>
public class EnumStringConverterFactory : IStringConverterFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumStringConverterFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public EnumStringConverterFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public bool ShouldHandle(Type type)
        => type.IsEnum;

    /// <inheritdoc />
    public Result<IStringConverter> CreateTypeSerializer(Type type)
    {
        var underlyingType = type.GetEnumUnderlyingType();
        var serializerType = typeof(EnumStringConverter<,>).MakeGenericType(type, underlyingType);

        try
        {
            return Result<IStringConverter>.FromSuccess
                ((IStringConverter)ActivatorUtilities.CreateInstance(_serviceProvider, serializerType));
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <inheritdoc />
    public Result<IStringConverter<T>> CreateTypeSerializer<T>()
        => CreateTypeSerializer(typeof(T)).Cast<IStringConverter<T>, IStringConverter>();
}