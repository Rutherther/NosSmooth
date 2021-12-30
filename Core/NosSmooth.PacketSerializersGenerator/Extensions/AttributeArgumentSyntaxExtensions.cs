//
//  AttributeArgumentSyntaxExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NosSmooth.PacketSerializersGenerator.Extensions;

/// <summary>
/// Extension methods for <see cref="AttributeArgumentSyntax"/>.
/// </summary>
public static class AttributeArgumentSyntaxExtensions
{
    /// <summary>
    /// Get the value of the argument.
    /// </summary>
    /// <param name="attributeArgument">The attribute argument.</param>
    /// <param name="semanticModel">The semantic model containing the attribute argument info.</param>
    /// <returns>The value.</returns>
    public static object? GetValue(this AttributeArgumentSyntax attributeArgument, SemanticModel semanticModel)
    {
        var value = semanticModel.GetConstantValue(attributeArgument.Expression);
        if (!value.HasValue)
        {
            return null;
        }

        return value;
    }
}