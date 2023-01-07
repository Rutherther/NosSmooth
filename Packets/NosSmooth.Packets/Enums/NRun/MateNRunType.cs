//
//  MateNRunType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS1591
namespace NosSmooth.Packets.Enums.NRun;

/// <summary>
/// A subtype for <see cref="NRunType"/>.Mate.
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Unknown yet.")]
public enum MateNRunType
{
    Company = 2,
    Stay = 3,
    RequestPetSendBack = 4,
    TriggerPetSendBack = 5,
    KickPet = 6,
    TriggerSummon = 7,
    Summon = 9
}