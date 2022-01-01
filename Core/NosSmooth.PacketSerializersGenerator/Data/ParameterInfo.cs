//
//  ParameterInfo.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NosSmooth.PacketSerializersGenerator.Data;

/// <summary>
/// Information about a parameter of a packet constructor.
/// </summary>
/// <param name="Parameter">The parameter's syntax.</param>
/// <param name="Type">The type of the parameter.</param>
/// <param name="Nullable">Whether the parameter type is nullable.</param>
/// <param name="Attributes">The list of all of the attribute on the parameter that are used for the generation of serializers.</param>
/// <param name="Name"></param>
/// <param name="ConstructorIndex"></param>
/// <param name="PacketIndex"></param>
public record ParameterInfo
(
    ParameterSyntax Parameter,
    ITypeSymbol Type,
    bool Nullable,
    IReadOnlyList<AttributeInfo> Attributes,
    string Name,
    int ConstructorIndex,
    int PacketIndex
);