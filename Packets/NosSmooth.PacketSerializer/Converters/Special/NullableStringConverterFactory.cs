//
//  NullableStringConverterFactory.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Packets.Converters.Special.Converters;
using NosSmooth.Packets.Extensions;
using NosSmooth.PacketSerializer.Abstractions;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Special;

/// <inheritdoc />
public class NullableStringConverterFactory : IStringConverterFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableStringConverterFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public NullableStringConverterFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public bool ShouldHandle(Type type)
        => Nullable.GetUnderlyingType(type) != null;

    /// <inheritdoc />
    public Result<IStringConverter> CreateTypeSerializer(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType is null)
        {
            throw new InvalidOperationException("Accepts only nullable types.");
        }

        var nullableConverterType = typeof(NullableStringConverter<>).MakeGenericType(underlyingType);
        try
        {
            return Result<IStringConverter>
                .FromSuccess
                    ((IStringConverter)ActivatorUtilities.CreateInstance(_serviceProvider, nullableConverterType));
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <inheritdoc />
    public Result<IStringConverter<T>> CreateTypeSerializer<T>()
    {
        return CreateTypeSerializer(typeof(T))
            .Cast<IStringConverter<T>, IStringConverter>();
    }
}