//
//  RaidBfPacketType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using NosSmooth.Packets.Server.Raids;
#pragma warning disable CS1591

namespace NosSmooth.Packets.Enums.Raids;

/// <summary>
/// A type of <see cref="RaidBfPacket"/>.
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Self-explanatory.")]
public enum RaidBfPacketType
{
    MissionStarted = 0,
    MissionCleared = 1,
    TimeUp = 2,
    LeaderDied = 3,
    NoLivesLeft = 4,
    MissionFailed = 5
}