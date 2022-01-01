//
//  NullableTypeConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Special;

/// <inheritdoc />
public class NullableTypeConverter : ISpecialTypeConverter
{
    private readonly ITypeConverterRepository _typeConverterRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableTypeConverter"/> class.
    /// </summary>
    /// <param name="typeConverterRepository">The type converter repository.</param>
    public NullableTypeConverter(ITypeConverterRepository typeConverterRepository)
    {
        _typeConverterRepository = typeConverterRepository;
    }

    /// <inheritdoc />
    public bool ShouldHandle(Type type)
        => Nullable.GetUnderlyingType(type) != null;

    /// <inheritdoc />
    public Result<object?> Deserialize(Type type, PacketStringEnumerator stringEnumerator)
        => _typeConverterRepository.Deserialize(Nullable.GetUnderlyingType(type)!, stringEnumerator);

    /// <inheritdoc />
    public Result Serialize(Type type, object? obj, PacketStringBuilder builder)
        => _typeConverterRepository.Serialize(Nullable.GetUnderlyingType(type)!, obj, builder);
}