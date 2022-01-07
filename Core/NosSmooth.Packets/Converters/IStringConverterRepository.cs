//
//  IStringConverterRepository.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.Packets.Converters.Special;
using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Converters;

/// <summary>
/// Repository for <see cref="IStringConverter"/>.
/// </summary>
public interface IStringConverterRepository
{
    /// <summary>
    /// Gets the type converter for the given type.
    /// </summary>
    /// <param name="type">The type to find converter for.</param>
    /// <returns>The type converter or an error.</returns>
    public Result<IStringConverter> GetTypeConverter(Type type);

    /// <summary>
    /// Gets the type converter for the given type.
    /// </summary>
    /// <typeparam name="TParseType">The type to find converter for.</typeparam>
    /// <returns>The type converter or an error.</returns>
    public Result<IStringConverter<TParseType>> GetTypeConverter<TParseType>();
}