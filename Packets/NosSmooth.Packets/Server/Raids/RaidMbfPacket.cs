//
//  RaidMbfPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Raids;

/// <summary>
/// Raid state of lives, lockers.
/// </summary>
/// <param name="MonsterLockerInitial"></param>
/// <param name="MonsterLockerCurrent"></param>
/// <param name="ButtonLockerInitial"></param>
/// <param name="ButtonLockerCurrent"></param>
/// <param name="CurrentLives"></param>
/// <param name="InitialLives"></param>
[PacketHeader("raidmbf", PacketSource.Server)]
[GenerateSerializer(true)]
public record RaidMbfPacket
(
    [PacketIndex(0)]
    short MonsterLockerInitial,
    [PacketIndex(1)]
    short MonsterLockerCurrent,
    [PacketIndex(2)]
    short ButtonLockerInitial,
    [PacketIndex(3)]
    short ButtonLockerCurrent,
    [PacketIndex(4)]
    short CurrentLives,
    [PacketIndex(5)]
    short InitialLives
) : IPacket;