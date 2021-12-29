//
//  ITypeConverterRepository.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.Packets.Converters.Special;
using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Converters;

/// <summary>
/// Repository for <see cref="ITypeConverter"/>.
/// </summary>
public interface ITypeConverterRepository
{
    /// <summary>
    /// Gets the type converter for the given type.
    /// </summary>
    /// <param name="type">The type to find converter for.</param>
    /// <returns>The type converter or an error.</returns>
    public Result<ITypeConverter> GetTypeConverter(Type type);

    /// <summary>
    /// Gets the type converter for the given type.
    /// </summary>
    /// <typeparam name="TParseType">The type to find converter for.</typeparam>
    /// <returns>The type converter or an error.</returns>
    public Result<ITypeConverter<TParseType>> GetTypeConverter<TParseType>();

    /// <summary>
    /// Convert the data from the enumerator to the given type.
    /// </summary>
    /// <param name="parseType">The type of the object to serialize.</param>
    /// <param name="stringEnumerator">The packet string enumerator with the current position.</param>
    /// <returns>The parsed object or an error.</returns>
    public Result<object?> Deserialize(Type parseType, PacketStringEnumerator stringEnumerator);

    /// <summary>
    /// Serializes the given object to string by appending to the packet string builder.
    /// </summary>
    /// <param name="parseType">The type of the object to serialize.</param>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="builder">The string builder to append to.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result Serialize(Type parseType, object obj, PacketStringBuilder builder);

    /// <summary>
    /// Convert the data from the enumerator to the given type.
    /// </summary>
    /// <param name="stringEnumerator">The packet string enumerator with the current position.</param>
    /// <typeparam name="TParseType">The type of the object to serialize.</typeparam>
    /// <returns>The parsed object or an error.</returns>
    public Result<TParseType?> Deserialize<TParseType>(PacketStringEnumerator stringEnumerator);

    /// <summary>
    /// Serializes the given object to string by appending to the packet string builder.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="builder">The string builder to append to.</param>
    /// <typeparam name="TParseType">The type of the object to deserialize.</typeparam>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result Serialize<TParseType>(TParseType obj, PacketStringBuilder builder);
}