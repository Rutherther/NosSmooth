//
//  AttributeListSyntaxExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NosSmooth.PacketSerializersGenerator.Extensions;

/// <summary>
/// Extension methods for <see cref="AttributeListSyntax"/>.
/// </summary>
public static class AttributeListSyntaxExtensions
{
    /// <summary>
    /// Whether the attribute list contains the attribute with the given full name.
    /// </summary>
    /// <param name="attributeList">The list of the attributes.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="attributeFullName">The full name of the attribute.</param>
    /// <returns>Whether the attribute is present.</returns>
    public static bool ContainsAttribute(this AttributeListSyntax attributeList, SemanticModel semanticModel, string attributeFullName)
    {
        return attributeList.Attributes.Any(x => Regex.IsMatch(attributeFullName, semanticModel.GetTypeInfo(x).Type?.ToString()!));
    }
}