//
//  QuestGoalType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS1591
namespace NosSmooth.Packets.Enums.Quests;

/// <summary>
/// A goal type of a quest.
/// </summary>
/// <remarks>
/// Says when the reward may be obtained.
/// </remarks>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Too many fields that are self-explanatory.")]
public enum QuestGoalType
{
    Hunt = 1,
    SpecialCollect = 2,
    CollectInRaid = 3,
    Brings = 4,
    CaptureWithoutGettingTheMonster = 5,
    Capture = 6,
    TimesSpace = 7,
    Product = 8,
    NumberOfKill = 9,
    TargetReput = 10,
    TsPoint = 11,
    Dialog1 = 12,
    CollectInTs = 13, // Collect in TimeSpace
    Required = 14,
    Wear = 15,
    Needed = 16,
    Collect = 17, // Collect basic items & quests items
    TransmitGold = 18,
    GoTo = 19,
    CollectMapEntity = 20, // Collect from map entity ( Ice Flower / Water )
    Use = 21,
    Dialog2 = 22,
    UnKnow = 23,
    Inspect = 24,
    WinRaid = 25,
    FlowerQuest = 26,
}