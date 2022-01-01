//
//  AttributeInfoExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                return (TValue?)(object?)value.VisualValue;
            }

            return (TValue?)value.RealValue;
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
            return (TValue?)(object?)value?.VisualValue;
        }

        return (TValue?)value.RealValue;
    }

    /// <summary>
    /// Gets visual values of params parameters in the constructor.
    /// </summary>
    /// <param name="attributeInfo">The attribute info.</param>
    /// <param name="startingIndex">The index the values start at.</param>
    /// <returns>A list containing all the values.</returns>
    public static IReadOnlyList<string> GetParamsVisualValues(this AttributeInfo attributeInfo, int startingIndex)
    {
        if (attributeInfo.IndexedAttributeArguments.Count - 1 < startingIndex)
        {
            return Array.Empty<string>();
        }

        if (attributeInfo.IndexedAttributeArguments[startingIndex].IsArray)
        {
            return attributeInfo.IndexedAttributeArguments[startingIndex].GetArrayVisualValues();
        }

        return attributeInfo.IndexedAttributeArguments
            .Skip(startingIndex)
            .Select(x => x.VisualValue)
            .ToArray();
    }

    /// <summary>
    /// Gets the visual values of the array.
    /// </summary>
    /// <param name="attributeArgumentInfo">The attribute argument.</param>
    /// <returns>The list of the elements.</returns>
    public static IReadOnlyList<string> GetArrayVisualValues(this AttributeArgumentInfo attributeArgumentInfo)
    {
        var arrayCreation = attributeArgumentInfo.Argument.Expression as ArrayCreationExpressionSyntax;
        if (arrayCreation is null)
        {
            throw new ArgumentException($"The given attribute argument is not an array creation.", nameof(attributeArgumentInfo));
        }

        if (arrayCreation.Initializer is null)
        {
            return Array.Empty<string>();
        }

        return arrayCreation.Initializer.Expressions
            .Select(x => x.ToString())
            .ToArray();
    }
}