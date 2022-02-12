//
//  MlInfoBrPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.Miniland;

/// <summary>
/// Miniland info packet. For minilands not owned by the playing character.
/// </summary>
/// <param name="MinilandMusicId">The id of the music. 3800 by default.</param>
/// <param name="OwnerName">The name of the owner.</param>
/// <param name="DailyVisitCount">The number of daily visits.</param>
/// <param name="VisitCount">The number of total visits.</param>
/// <param name="Unknown">Unknown TODO.</param>
/// <param name="MinilandMessage">The welcome message.</param>
[PacketHeader("mlinfobr", PacketSource.Server)]
[GenerateSerializer(true)]
public record MlInfoBrPacket
(
    [PacketIndex(0)]
    short MinilandMusicId,
    [PacketIndex(1)]
    NameString OwnerName,
    [PacketIndex(2)]
    int DailyVisitCount,
    [PacketIndex(3)]
    int VisitCount,
    [PacketIndex(4)]
    byte Unknown,
    [PacketGreedyIndex(5)]
    string MinilandMessage
) : IPacket;