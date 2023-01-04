//
//  QstListSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Quests;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Quests;

/// <summary>
/// A sub packet of <see cref="QstListPacket"/>
/// containing information about an active quest.
/// </summary>
/// <param name="QuestNumber">The number of the quest.</param>
/// <param name="InfoId">The info id of the quest to obtain it from .NOS files.</param>
/// <param name="InfoId2">Unknown TODO.</param>
/// <param name="GoalType">The quest goal type.</param>
/// <param name="Objective1SubPacket">The first objective of the quest.</param>
/// <param name="IsQuestFinished"></param>
/// <param name="Objective2SubPacket">The second objective of the quest.</param>
/// <param name="Objective3SubPacket">The third objective of the quest.</param>
/// <param name="Objective4SubPacket">The fourth objective of the quest.</param>
/// <param name="Objective5SubPacket">The fifth objective of the quest.</param>
/// <param name="ShowDialog">Whether to show dialog at start of the quest.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record QstListSubPacket
(
    [PacketIndex(0)]
    short QuestNumber,
    [PacketIndex(1)]
    short InfoId,
    [PacketIndex(2)]
    short InfoId2,
    [PacketIndex(3)]
    QuestGoalType GoalType,
    [PacketIndex(4, InnerSeparator = '.')]
    QstListObjectiveSubPacket Objective1SubPacket,
    [PacketIndex(5)]
    bool IsQuestFinished,
    [PacketIndex(6, InnerSeparator = '.')]
    QstListObjectiveSubPacket Objective2SubPacket,
    [PacketIndex(7, InnerSeparator = '.')]
    QstListObjectiveSubPacket Objective3SubPacket,
    [PacketIndex(8, InnerSeparator = '.')]
    QstListObjectiveSubPacket Objective4SubPacket,
    [PacketIndex(9, InnerSeparator = '.')]
    QstListObjectiveSubPacket Objective5SubPacket,
    [PacketIndex(10)]
    bool ShowDialog
) : IPacket;