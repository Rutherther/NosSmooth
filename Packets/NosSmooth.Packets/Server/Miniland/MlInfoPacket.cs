//
//  MlInfoPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.Miniland;

/// <summary>
/// Miniland info packet. For miniland owned by the playing character.
/// </summary>
/// <param name="MinilandMusicId">The id of the music. 3800 by default.</param>
/// <param name="MinilandPoints">The points of the miniland.</param>
/// <param name="Unknown">Unknown TODO.</param>
/// <param name="DailyVisitCount">The number of daily visits.</param>
/// <param name="VisitCount">The number of total visits.</param>
/// <param name="Unknown1">Unknown TODO.</param>
/// <param name="MinilandState">The state of the miniland.</param>
/// <param name="MinilandMusicName">The name of the miniland music.</param>
/// <param name="MinilandMessage">The welcome message.</param>
[PacketHeader("mlinfo", PacketSource.Server)]
[GenerateSerializer(true)]
public record MlInfoPacket
(
    [PacketIndex(0)]
    short MinilandMusicId,
    [PacketIndex(1)]
    long MinilandPoints,
    [PacketIndex(2)]
    byte Unknown,
    [PacketIndex(3)]
    int DailyVisitCount,
    [PacketIndex(4)]
    int VisitCount,
    [PacketIndex(5)]
    byte Unknown1,
    [PacketIndex(6)]
    MinilandState MinilandState,
    [PacketIndex(7)]
    NameString MinilandMusicName,
    [PacketGreedyIndex(8)]
    string MinilandMessage
) : IPacket;