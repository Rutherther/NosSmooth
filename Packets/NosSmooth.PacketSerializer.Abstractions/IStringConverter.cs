﻿//
//  IStringConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.PacketSerializer.Abstractions;

/// <summary>
/// Base type for converting types.
/// </summary>
public interface IStringConverter
{
    /// <summary>
    /// Convert the data from the enumerator to the given type.
    /// </summary>
    /// <param name="stringEnumerator">The packet string enumerator with the current position.</param>
    /// <param name="options">The deserialization options.</param>
    /// <returns>The parsed object or an error.</returns>
    public Result<object?> Deserialize(ref PacketStringEnumerator stringEnumerator, DeserializeOptions options);

    /// <summary>
    /// Serializes the given object to string by appending to the packet string builder.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="builder">The string builder to append to.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result Serialize(object? obj, ref PacketStringBuilder builder);
}

/// <summary>
/// Converts string to an object.
/// </summary>
/// <remarks>
/// Used for converting packets.
/// </remarks>
/// <typeparam name="TParseType">The type that can be parsed.</typeparam>
public interface IStringConverter<TParseType> : IStringConverter
{
    /// <summary>
    /// Convert the data from the enumerator to the given type.
    /// </summary>
    /// <param name="stringEnumerator">The packet string enumerator with the current position.</param>
    /// <param name="options">The deserialization options.</param>
    /// <returns>The parsed object or an error.</returns>
    public new Result<TParseType?> Deserialize(ref PacketStringEnumerator stringEnumerator, DeserializeOptions options);

    /// <summary>
    /// Serializes the given object to string by appending to the packet string builder.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="builder">The string builder to append to.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result Serialize(TParseType? obj, ref PacketStringBuilder builder);
}