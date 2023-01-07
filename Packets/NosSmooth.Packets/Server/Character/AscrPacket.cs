//
//  AscrPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Packets;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Character;

/// <summary>
/// Character statistics (kills, deaths) packet.
/// </summary>
/// <param name="CurrentKill">The current kill count.</param>
/// <param name="CurrentDie">The current die count.</param>
/// <param name="CurrentTc">The current tc count.</param>
/// <param name="ArenaKill">The current arena kill count.</param>
/// <param name="ArenaDie">The current arena die count.</param>
/// <param name="ArenaTc">The current arena tc count.</param>
/// <param name="KillGroup">Unknown TODO.</param>
/// <param name="DieGroup">Unknown TODO.</param>
/// <param name="Type">The type of the ascr packet.</param>
[PacketHeader("ascr", PacketSource.Server)]
[GenerateSerializer(true)]
public record AscrPacket
(
    [PacketIndex(0)]
    int CurrentKill,
    [PacketIndex(1)]
    int CurrentDie,
    [PacketIndex(2)]
    int CurrentTc,
    [PacketIndex(3)]
    int ArenaKill,
    [PacketIndex(4)]
    int ArenaDie,
    [PacketIndex(5)]
    int ArenaTc,
    [PacketIndex(6)]
    int KillGroup,
    [PacketIndex(7)]
    int DieGroup,
    [PacketIndex(8)]
    AscrPacketType Type
) : IPacket;