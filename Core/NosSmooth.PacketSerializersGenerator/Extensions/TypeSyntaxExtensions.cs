//
//  TypeSyntaxExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NosSmooth.PacketSerializersGenerator.Extensions;

/// <summary>
/// Extension methods for <see cref="TypeSyntax"/>.
/// </summary>
public static class TypeSyntaxExtensions
{
    /// <summary>
    /// Gets whether the type syntax is nullable.
    /// </summary>
    /// <param name="typeSyntax">The type syntax.</param>
    /// <returns>Whether the type syntax is nullable.</returns>
    public static bool IsNullable(this TypeSyntax typeSyntax)
    {
        return typeSyntax is NullableTypeSyntax || typeSyntax.ToString().EndsWith("?");
    }

    /// <summary>
    /// Gets the type name with ? if it is nullable.
    /// </summary>
    /// <param name="typeSyntax">The type.</param>
    /// <returns>The actual name.</returns>
    public static string GetActualType(this TypeSyntax typeSyntax)
    {
        if (typeSyntax.IsNullable())
        {
            return typeSyntax.ToString().TrimEnd('?') + '?';
        }

        return typeSyntax.ToString();
    }
}