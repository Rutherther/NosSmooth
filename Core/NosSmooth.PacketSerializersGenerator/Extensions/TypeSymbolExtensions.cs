//
//  TypeSymbolExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NosSmooth.PacketSerializersGenerator.Extensions;

/// <summary>
/// Extension methods for <see cref="ITypeSymbol"/>.
/// </summary>
public static class TypeSymbolExtensions
{
    /// <summary>
    /// Gets whether the type symbol is nullable.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns>Whether the type symbol is nullable. Returns null if not possible to determine.</returns>
    public static bool? IsNullable(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol.NullableAnnotation == NullableAnnotation.Annotated || typeSymbol.Name == "Nullable")
        {
            return true;
        }

        if (typeSymbol.IsValueType)
        {
            return false;
        }

        if (typeSymbol.ToString().EndsWith("?"))
        {
            return true;
        }

        // cannot determine if not nullable from reference type.
        return null;
    }

    /// <summary>
    /// Gets the type name with ? if it is nullable.
    /// </summary>
    /// <param name="typeSymbol">The type.</param>
    /// <returns>The actual name.</returns>
    public static string GetActualType(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol.IsNullable() ?? false)
        {
            return typeSymbol.ToString().TrimEnd('?') + '?';
        }

        return typeSymbol.ToString();
    }
}