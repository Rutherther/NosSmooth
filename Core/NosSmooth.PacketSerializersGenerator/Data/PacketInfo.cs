//
//  PacketInfo.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NosSmooth.PacketSerializersGenerator.Data;

/// <summary>
/// Contains information about a packet record syntax.
/// </summary>
/// <param name="Compilation">The compilation of the generator.</param>
/// <param name="PacketRecord">The packet record declaration.</param>
/// <param name="SemanticModel">The semantic model the packet is in.</param>
/// <param name="Parameters">The parsed parameters of the packet.</param>
/// <param name="Namespace">The namespace of the packet record.</param>
/// <param name="Name">The name of the packet.</param>
public record PacketInfo
(
    Compilation Compilation,
    AttributeInfo GenerateAttribute,
    RecordDeclarationSyntax PacketRecord,
    SemanticModel SemanticModel,
    Parameters Parameters,
    string Namespace,
    string Name
);