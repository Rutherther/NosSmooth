//
//  TypeSymbolExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

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

        // cannot determine if not nullable from reference type.
        return null;
    }
}