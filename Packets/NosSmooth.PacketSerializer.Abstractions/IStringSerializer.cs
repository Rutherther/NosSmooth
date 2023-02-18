//
//  IStringSerializer.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.PacketSerializer.Abstractions;

/// <summary>
/// Serializer of values from NosTale packet strings.
/// </summary>
public interface IStringSerializer
{
    /// <summary>
    /// Convert the data from the enumerator to the given type.
    /// </summary>
    /// <param name="parseType">The type of the object to serialize.</param>
    /// <param name="stringEnumerator">The packet string enumerator with the current position.</param>
    /// <param name="options">The deserialization options.</param>
    /// <returns>The parsed object or an error.</returns>
    public Result<object?> Deserialize(Type parseType, in PacketStringEnumerator stringEnumerator, DeserializeOptions options);

    /// <summary>
    /// Serializes the given object to string by appending to the packet string builder.
    /// </summary>
    /// <param name="parseType">The type of the object to serialize.</param>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="builder">The string builder to append to.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result Serialize(Type parseType, object? obj, in PacketStringBuilder builder);

    /// <summary>
    /// Convert the data from the enumerator to the given type.
    /// </summary>
    /// <param name="stringEnumerator">The packet string enumerator with the current position.</param>
    /// <typeparam name="TParseType">The type of the object to serialize.</typeparam>
    /// <param name="options">The deserialization options.</param>
    /// <returns>The parsed object or an error.</returns>
    public Result<TParseType?> Deserialize<TParseType>(in PacketStringEnumerator stringEnumerator, DeserializeOptions options);

    /// <summary>
    /// Serializes the given object to string by appending to the packet string builder.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="builder">The string builder to append to.</param>
    /// <typeparam name="TParseType">The type of the object to deserialize.</typeparam>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result Serialize<TParseType>(TParseType? obj, in PacketStringBuilder builder);
}