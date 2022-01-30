//
//  BagType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS1591
namespace NosSmooth.Data.Abstractions.Enums;

/// <summary>
/// The type of a bag the item belongs to.
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Self-explanatory.")]
public enum BagType
{
    Equipment = 0,
    Main = 1,
    Etc = 2,
    Miniland = 3,
    Specialist = 6,
    Costume = 7
}