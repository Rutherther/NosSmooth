//
//  ListStringConverterFactory.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.PacketSerializer.Abstractions;
using NosSmooth.PacketSerializer.Converters.Special.Converters;
using NosSmooth.PacketSerializer.Extensions;
using Remora.Results;

namespace NosSmooth.PacketSerializer.Converters.Special;

/// <summary>
/// Converts lists.
/// </summary>
public class ListStringConverterFactory : IStringConverterFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListStringConverterFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public ListStringConverterFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public bool ShouldHandle(Type type)
        => type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type);

    /// <inheritdoc />
    public Result<IStringConverter> CreateTypeSerializer(Type type)
    {
        var elementType = type.GetElementType() ?? type.GetGenericArguments()[0];
        var converterType = typeof(ListStringConverter<>).MakeGenericType(elementType);
        try
        {
            return Result<IStringConverter>
                .FromSuccess((IStringConverter)ActivatorUtilities.CreateInstance(_serviceProvider, converterType));
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