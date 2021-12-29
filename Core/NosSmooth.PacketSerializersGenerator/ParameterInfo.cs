//
//  ParameterInfo.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NosSmooth.PacketSerializersGenerator;

/// <summary>
/// Information about a parameter of a packet constructor.
/// </summary>
/// <param name="Compilation"></param>
/// <param name="Parameter"></param>
/// <param name="Attribute"></param>
/// <param name="IndexedAttributeArguments"></param>
/// <param name="NamedAttributeArguments"></param>
/// <param name="Name"></param>
/// <param name="ConstructorIndex"></param>
/// <param name="PacketIndex"></param>
public record ParameterInfo
(
    Compilation Compilation,
    ParameterSyntax Parameter,
    AttributeSyntax Attribute,
    IReadOnlyList<object?> IndexedAttributeArguments,
    IReadOnlyDictionary<string, object?> NamedAttributeArguments,
    string Name,
    int ConstructorIndex,
    int PacketIndex
)
{
    /// <summary>
    /// Gets or sets if this parameter is the last one.
    /// </summary>
    public bool IsLast { get; set; }
}