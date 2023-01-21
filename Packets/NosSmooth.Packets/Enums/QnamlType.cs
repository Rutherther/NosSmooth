//
//  QnamlType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using NosSmooth.Packets.Server.UI;
#pragma warning disable CS1591

namespace NosSmooth.Packets.Enums;

/// <summary>
/// Type of <see cref="QnamlPacket"/>, <see cref="Qnamli2Packet"/>.
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "TODO")]
public enum QnamlType
{
    Dialog = 0,
    InstantCombat = 1,
    Icebreaker = 2,
    FamilyMemberCalled = 3,
    SheepRaid = 4,
    TournamentEvent = 5,
    MeteorInvRaid = 6,
    BattleRoyale = 7,
    RainbowBattle = 8,
    BushiKingRaid = 9,
    Raid = 100
}