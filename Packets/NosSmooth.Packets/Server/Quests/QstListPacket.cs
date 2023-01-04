//
//  QstListPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Quests;

/// <summary>
/// A list of quests
/// that are currently active.
/// </summary>
/// <param name="QuestSubPackets">The list of the active quests.</param>
[PacketHeader("qstlist", PacketSource.Server)]
[GenerateSerializer(true)]
public record QstListPacket
(
    [PacketListIndex(0, InnerSeparator = '.', ListSeparator = ' ')]
    IReadOnlyList<QstListSubPacket> QuestSubPackets
) : IPacket;