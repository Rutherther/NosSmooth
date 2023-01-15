//
//  SpeakType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using NosSmooth.Packets.Server.Chat;
#pragma warning disable CS1591

namespace NosSmooth.Packets.Enums.Chat;

/// <summary>
/// A type of <see cref="SpkPacket"/>.
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Self-explanatory.")]
public enum SpeakType
{
    Normal = 0,
    Family = 1,
    Group = 3,
    Player = 5,
    TimeSpace = 7,
    GameMaster = 15
}