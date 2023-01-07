//
//  OptionalWrapperConverterFactory.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.PacketSerializer.Abstractions;
using NosSmooth.PacketSerializer.Converters.Special;
using NosSmooth.PacketSerializer.Extensions;
using Remora.Results;

namespace NosSmooth.PacketSerializer.Converters.Common;

/// <summary>
/// Converts <see cref="OptionalWrapper{T}"/>.
/// </summary>
public class OptionalWrapperConverterFactory : IStringConverterFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionalWrapperConverterFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public OptionalWrapperConverterFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public bool ShouldHandle(Type type)
        => type.GetGenericTypeDefinition() == typeof(OptionalWrapper<>);

    /// <inheritdoc />
    public Result<IStringConverter> CreateTypeSerializer(Type type)
    {
        var underlyingType = type.GetGenericArguments()[0];
        var serializerType = typeof(OptionalWrapperConverter<>).MakeGenericType(underlyingType);

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