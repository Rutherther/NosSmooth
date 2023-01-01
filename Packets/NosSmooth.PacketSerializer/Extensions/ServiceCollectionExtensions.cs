//
//  ServiceCollectionExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Packets;
using NosSmooth.PacketSerializer.Abstractions;
using NosSmooth.PacketSerializer.Converters;
using NosSmooth.PacketSerializer.Converters.Basic;
using NosSmooth.PacketSerializer.Converters.Common;
using NosSmooth.PacketSerializer.Converters.Packets;
using NosSmooth.PacketSerializer.Converters.Special;
using NosSmooth.PacketSerializer.Packets;

namespace NosSmooth.PacketSerializer.Extensions;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add packet serialization classes.
    /// </summary>
    /// <remarks>
    /// All generic implementations of IStringConverter the class
    /// implements will be registered.
    /// </remarks>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddPacketSerialization(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<IStringConverterRepository, StringConverterRepository>()
            .AddSingleton<IStringSerializer, StringSerializer>()
            .AddSingleton<IPacketSerializer, PacketSerializer>()
            .AddSingleton<IPacketTypesRepository, PacketTypesRepository>()
            .AddGeneratedSerializers(typeof(IPacket).Assembly)
            .AddBasicConverters();
    }

    /// <summary>
    /// Adds all generated serializers from the given assembly.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="assembly">The assembly.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddGeneratedSerializers(this IServiceCollection serviceCollection, Assembly assembly)
    {
        var types = assembly.GetExportedTypes()
            .Where(x => x.Namespace?.Contains("Generated") ?? false)
            .Where(x => x.GetInterfaces().Any(
                i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IStringConverter<>)
            ));
        foreach (var type in types)
        {
            serviceCollection.AddStringConverter(type);
        }

        return serviceCollection;
    }

    /// <summary>
    /// Adds basic converters for int, uint, short, ushort, long, ulong, char, string
    /// and special converters for lists and enums.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddBasicConverters(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddStringConverterFactory<ListStringConverterFactory>()
            .AddStringConverterFactory<NullableStringConverterFactory>()
            .AddStringConverterFactory<EnumStringConverterFactory>()
            .AddStringConverter<IntStringConverter>()
            .AddStringConverter<BoolStringConverter>()
            .AddStringConverter<UIntStringConverter>()
            .AddStringConverter<ShortStringConverter>()
            .AddStringConverter<UShortStringConverter>()
            .AddStringConverter<ByteStringConverter>()
            .AddStringConverter<ULongStringConverter>()
            .AddStringConverter<LongStringConverter>()
            .AddStringConverter<StringTypeConverter>()
            .AddStringConverter<NameStringConverter>()
            .AddStringConverter<CharStringConverter>()
            .AddStringConverter<UpgradeRareSubPacketConverter>();
    }

    /// <summary>
    /// Add generic type converter.
    /// </summary>
    /// <remarks>
    /// All generic implementations of IStringConverter the class
    /// implements will be registered.
    /// </remarks>
    /// <param name="serviceCollection">The service collection.</param>
    /// <typeparam name="TConverter">The type of the converter.</typeparam>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddStringConverter<TConverter>(this IServiceCollection serviceCollection)
        where TConverter : IStringConverter
        => serviceCollection.AddStringConverter(typeof(TConverter));

    /// <summary>
    /// Add generic type converter.
    /// </summary>
    /// <remarks>
    /// All generic implementations of IStringConverter the class
    /// implements will be registered.
    /// </remarks>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="converterType">The type of the converter.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddStringConverter(this IServiceCollection serviceCollection, Type converterType)
    {
        if (!converterType.GetInterfaces().Any(
                i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IStringConverter<>)
            ))
        {
            throw new ArgumentException(
                $"{nameof(converterType)} should implement IStringConverter.",
                nameof(converterType));
        }

        var handlerTypeInterfaces = converterType.GetInterfaces();
        var handlerInterfaces = handlerTypeInterfaces.Where
        (
            r => r.IsGenericType && r.GetGenericTypeDefinition() == typeof(IStringConverter<>)
        );

        foreach (var handlerInterface in handlerInterfaces)
        {
            serviceCollection.AddSingleton(handlerInterface, converterType);
        }

        return serviceCollection;
    }

    /// <summary>
    /// Add the specified converter as a special converter.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <typeparam name="TConverterFactory">The type to add as a special converter.</typeparam>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddStringConverterFactory<TConverterFactory>(this IServiceCollection serviceCollection)
        where TConverterFactory : class, IStringConverterFactory
    {
        return serviceCollection.AddSingleton<IStringConverterFactory, TConverterFactory>();
    }
}