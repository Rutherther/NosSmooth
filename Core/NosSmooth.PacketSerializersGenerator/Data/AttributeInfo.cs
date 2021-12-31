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
    IReadOnlyList<AttributeArgumentInfo> IndexedAttributeArguments,
    IReadOnlyDictionary<string, AttributeArgumentInfo> NamedAttributeArguments
);

/// <summary>
/// The attribute argument information.
/// </summary>
/// <param name="Argument">The argument syntax.</param>
/// <param name="IsArray">Whether the attribute argument is an array.</param>
/// <param name="RealValue">The real parsed value of the argument.</param>
/// <param name="VisualValue">The visual value of the argument.</param>
public record AttributeArgumentInfo(AttributeArgumentSyntax Argument, bool IsArray, object? RealValue, string VisualValue);