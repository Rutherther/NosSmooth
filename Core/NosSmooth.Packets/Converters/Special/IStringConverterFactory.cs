//
//  IStringConverterFactory.cs
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
public interface IStringConverterFactory
{
    /// <summary>
    /// Whether this type converter should handle the given type.
    /// </summary>
    /// <param name="type">The type to handle.</param>
    /// <returns>Whether the type should be handled.</returns>
    public bool ShouldHandle(Type type);

    /// <summary>
    /// Create converter for the given type.
    /// </summary>
    /// <param name="type">The type to create converter for.</param>
    /// <returns>The type converter or an error.</returns>
    public Result<IStringConverter> CreateTypeSerializer(Type type);

    /// <summary>
    /// Create generic converter for the given type.
    /// </summary>
    /// <typeparam name="T">The type to create converter for.</typeparam>
    /// <returns>The type converter or an error.</returns>
    public Result<IStringConverter<T>> CreateTypeSerializer<T>();
}