//
//  QstListObjectiveSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Quests;

/// <summary>
/// A sub packet of <see cref="QstListPacket"/>
/// containing information about a quest
/// objective.
/// </summary>
/// <param name="CurrentCount"></param>
/// <param name="MaxCount"></param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record QstListObjectiveSubPacket
(
    [PacketIndex(0)]
    short CurrentCount,
    [PacketIndex(1)]
    short MaxCount
);