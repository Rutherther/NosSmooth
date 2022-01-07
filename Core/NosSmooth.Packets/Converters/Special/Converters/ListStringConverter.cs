//
//  ListStringConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Special.Converters;

/// <summary>
/// Converter for list types.
/// </summary>
/// <typeparam name="TGeneric">The generic type argument of the list.</typeparam>
public class ListStringConverter<TGeneric> : BaseStringConverter<IReadOnlyList<TGeneric>>
{
    private readonly IStringSerializer _serializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListStringConverter{TGeneric}"/> class.
    /// </summary>
    /// <param name="serializer">The string serializer.</param>
    public ListStringConverter(IStringSerializer serializer)
    {
        _serializer = serializer;

    }

    /// <inheritdoc />
    public override Result Serialize(IReadOnlyList<TGeneric>? obj, PacketStringBuilder builder)
    {
        if (obj is null)
        {
            builder.Append('-');
            return Result.FromSuccess();
        }

        foreach (var item in obj)
        {
            if (!builder.PushPreparedLevel())
            {
                return new ArgumentInvalidError(nameof(builder), "The string builder has to have a prepared level for all lists.");
            }

            var serializeResult = _serializer.Serialize(item, builder);
            builder.PopLevel();
            if (!serializeResult.IsSuccess)
            {
                return serializeResult;
            }
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public override Result<IReadOnlyList<TGeneric>?> Deserialize(PacketStringEnumerator stringEnumerator)
    {
        var list = new List<TGeneric>();

        while (!(stringEnumerator.IsOnLastToken() ?? false))
        {
            if (!stringEnumerator.PushPreparedLevel())
            {
                return new ArgumentInvalidError(nameof(stringEnumerator), "The string enumerator has to have a prepared level for all lists.");
            }

            var result = _serializer.Deserialize<TGeneric>(stringEnumerator);

            // If we know that we are not on the last token in the item level, just skip to the end of the item.
            // Note that if this is the case, then that means the converter is either corrupted
            // or the packet has more fields.
            while (stringEnumerator.IsOnLastToken() == false)
            {
                stringEnumerator.GetNextToken();
            }

            stringEnumerator.PopLevel();
            if (!result.IsSuccess)
            {
                return Result<IReadOnlyList<TGeneric>?>.FromError(new ListSerializerError(result, list.Count), result);
            }

            if (result.Entity is null)
            {
                return new DeserializedValueNullError(typeof(IReadOnlyList<TGeneric>));
            }

            list.Add(result.Entity);
        }

        return list;
    }
}