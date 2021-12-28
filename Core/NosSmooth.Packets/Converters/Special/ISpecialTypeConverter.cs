//
//  ISpecialTypeConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Special;

/// <summary>
/// Converts special types such as enums or lists.
/// </summary>
public interface ISpecialTypeConverter
{
    /// <summary>
    /// Whether this type converter should handle the given type.
    /// </summary>
    /// <param name="type">The type to handle.</param>
    /// <returns>Whether the type should be handled.</returns>
    public bool ShouldHandle(Type type);

    /// <summary>
    /// Deserialize the given string to the object.
    /// </summary>
    /// <param name="type">The type to deserialize.</param>
    /// <param name="stringEnumerator">The packets string enumerator.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result<object?> Deserialize(Type type, PacketStringEnumerator stringEnumerator);

    /// <summary>
    /// Deserialize the given object into string.
    /// </summary>
    /// <param name="type">The type to serialize.</param>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="builder">The packet string builder.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result Serialize(Type type, object? obj, PacketStringBuilder builder);
}