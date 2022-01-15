//
//  SyntaxNodeExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NosSmooth.PacketSerializersGenerator.Extensions;

/// <summary>
/// Extensions for <see cref="SyntaxNode"/>.
/// </summary>
public static class SyntaxNodeExtensions
{
    /// <summary>
    /// Gets the prefix of the given member (class or namespace).
    /// </summary>
    /// <param name="member">The member to get prefix of.</param>
    /// <returns>The full name.</returns>
    public static string GetPrefix(this SyntaxNode? member)
    {
        if (member is null)
        {
            return string.Empty;
        }

        StringBuilder sb = new StringBuilder();
        SyntaxNode node = member;

        while (node.Parent != null)
        {
            node = node.Parent;

            if (node is NamespaceDeclarationSyntax)
            {
                var namespaceDeclaration = (NamespaceDeclarationSyntax)node;

                sb.Insert(0, ".");
                sb.Insert(0, namespaceDeclaration.Name.ToString());
            }
            else if (node is FileScopedNamespaceDeclarationSyntax fileScopedNamespaceDeclarationSyntax)
            {
                sb.Insert(0, ".");
                sb.Insert(0, fileScopedNamespaceDeclarationSyntax.Name.ToString());
            }
            else if (node is ClassDeclarationSyntax)
            {
                var classDeclaration = (ClassDeclarationSyntax)node;

                sb.Insert(0, ".");
                sb.Insert(0, classDeclaration.Identifier.ToString());
            }
        }

        return sb.ToString().Trim('.');
    }
}