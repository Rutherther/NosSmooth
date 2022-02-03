//
//  PortalType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS1591
namespace NosSmooth.Packets.Enums;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600", MessageId = "Elements should be documented", Justification = "Should be self-explanatory.")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602", MessageId = "Enumeration items should be documented", Justification = "Should be self-explanatory.")]
public enum PortalType
{
    MapPortal = -1,
    TsNormal = 0, // same over >127 - sbyte
    Closed = 1,
    Open = 2,
    Miniland = 3,
    TsEnd = 4,
    TsEndClosed = 5,
    Exit = 6,
    ExitClosed = 7,
    Raid = 8,
    Effect = 9, // same as 13 - 19 and 20 - 126
    BlueRaid = 10,
    DarkRaid = 11,
    TimeSpace = 12,
    ShopTeleport = 20
}