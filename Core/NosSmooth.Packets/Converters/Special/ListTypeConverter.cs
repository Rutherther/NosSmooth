//
//  ListTypeConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Special;

/// <summary>
/// Converts lists.
/// </summary>
public class ListTypeConverter : ISpecialTypeConverter
{
    private readonly TypeConverterRepository _typeConverterRepository;
    private readonly ConcurrentDictionary<Type, Func<IEnumerable<object?>, object>> _fillFunctions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListTypeConverter"/> class.
    /// </summary>
    /// <param name="typeConverterRepository">The type converter repository.</param>
    public ListTypeConverter(TypeConverterRepository typeConverterRepository)
    {
        _typeConverterRepository = typeConverterRepository;
        _fillFunctions = new ConcurrentDictionary<Type, Func<IEnumerable<object?>, object>>();
    }

    /// <inheritdoc />
    public bool ShouldHandle(Type type)
        => type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type);

    /// <inheritdoc />
    public Result<object?> Deserialize(Type type, PacketStringEnumerator stringEnumerator)
    {
        var data = new List<object?>();
        var genericType = type.GetElementType() ?? type.GetGenericArguments()[0];

        do
        {
            if (!stringEnumerator.PushPreparedLevel())
            {
                return new ArgumentInvalidError(nameof(stringEnumerator), "The string enumerator has to have a prepared level for all lists.");
            }

            var result = _typeConverterRepository.Deserialize(genericType, stringEnumerator);

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
                return Result<object?>.FromError(new ListSerializerError(result, data.Count), result);
            }

            data.Add(result.Entity);
        }
        while (!(stringEnumerator.IsOnLastToken() ?? false));

        return _fillFunctions.GetOrAdd(genericType, GetAndFillListMethod)(data);
    }

    /// <inheritdoc />
    public Result Serialize(Type type, object? obj, PacketStringBuilder builder)
    {
        if (obj is null)
        {
            builder.Append("-");
            return Result.FromSuccess();
        }

        var items = (IEnumerable)obj;
        var genericType = type.GetElementType() ?? type.GetGenericArguments()[0];

        foreach (var item in items)
        {
            if (!builder.PushPreparedLevel())
            {
                return new ArgumentInvalidError(nameof(builder), "The string builder has to have a prepared level for all lists.");
            }

            var serializeResult = _typeConverterRepository.Serialize(genericType, item, builder);
            builder.ReplaceWithParentSeparator();
            builder.PopLevel();
            if (!serializeResult.IsSuccess)
            {
                return serializeResult;
            }
        }
        builder.ReplaceWithParentSeparator();

        return Result.FromSuccess();
    }

    // TODO: cache the functions?

    /// <summary>
    /// From https://stackoverflow.com/questions/35913495/moving-from-reflection-to-expression-tree.
    /// </summary>
    /// <param name="genericType">The generic type.</param>
    /// <returns>The function.</returns>
    private Func<IEnumerable<object?>, object> GetAndFillListMethod(Type genericType)
    {
        var listType = typeof(List<>);
        var listGenericType = listType.MakeGenericType(genericType);

        var values = Expression.Parameter(typeof(IEnumerable<object?>), "values");

        var ctor = listGenericType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null);

        // I prefer using Expression.Variable to Expression.Parameter
        // for internal variables
        var instance = Expression.Variable(listGenericType, "list");

        var assign = Expression.Assign(instance, Expression.New(ctor!));

        var addMethod = listGenericType.GetMethod("AddRange", new[] { typeof(IEnumerable<>).MakeGenericType(genericType) });

        // Enumerable.Cast<T>
        var castMethod = typeof(Enumerable).GetMethod("Cast", new[] { typeof(IEnumerable) })!.MakeGenericMethod(genericType);

        // For the parameters there is a params Expression[], so no explicit array necessary
        var castCall = Expression.Call(castMethod, values);
        var addCall = Expression.Call(instance, addMethod!, castCall);

        var block = Expression.Block(
            new[] { instance },
            assign,
            addCall,
            Expression.Convert(instance, typeof(object))
        );

        return (Func<IEnumerable<object?>, object>)Expression.Lambda(block, values).Compile();
    }
}