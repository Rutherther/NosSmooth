//
//  AttributeInfoExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using NosSmooth.PacketSerializersGenerator.Data;

namespace NosSmooth.PacketSerializersGenerator.Extensions;

/// <summary>
/// Extension methods for <see cref="AttributeInfo"/>.
/// </summary>
public static class AttributeInfoExtensions
{
    /// <summary>
    /// Get value of a named parameter.
    /// </summary>
    /// <param name="attributeInfo">The attribute information.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="default">The default value to return if not found.</param>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <returns>The value of the attribute.</returns>
    public static TValue? GetNamedValue<TValue>(this AttributeInfo attributeInfo, string name, TValue? @default)
    {
        if (attributeInfo.NamedAttributeArguments.TryGetValue(name, out var value))
        {
            if (typeof(TValue) == typeof(string))
            {
                return (TValue?)(object?)value?.ToString();
            }

            return (TValue?)value;
        }

        return @default;
    }

    /// <summary>
    /// Get value of a named parameter.
    /// </summary>
    /// <param name="attributeInfo">The attribute information.</param>
    /// <param name="index">The index of the parameter.</param>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <returns>The value of the attribute.</returns>
    public static TValue? GetIndexedValue<TValue>(this AttributeInfo attributeInfo, int index)
    {
        var value = attributeInfo.IndexedAttributeArguments[index];

        if (typeof(TValue) == typeof(string))
        {
            return (TValue?)(object?)value?.ToString();
        }

        return (TValue?)value;
    }
}