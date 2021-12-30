//
//  AttributeInfo.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NosSmooth.PacketSerializersGenerator.Data;

/// <summary>
/// The attribute info.
/// </summary>
/// <param name="Attribute">The attribute syntax.</param>
/// <param name="FullName">The full name of the attribute containing namespace.</param>
/// <param name="IndexedAttributeArguments">The indexed arguments passed to the attribute.</param>
/// <param name="NamedAttributeArguments">The named arguments passed to the attribute.</param>
public record AttributeInfo
(
    AttributeSyntax Attribute,
    string FullName,
    IReadOnlyList<object?> IndexedAttributeArguments,
    IReadOnlyDictionary<string, object?> NamedAttributeArguments
);