//
//  PinitPlayerSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Entities;
using NosSmooth.Packets.Enums.Mates;
using NosSmooth.Packets.Enums.Players;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.Groups;

/// <summary>
/// A sub packet of <see cref="PinitPacket"/>
/// representing a player.
/// </summary>
/// <param name="GroupPosition">The position in the group.</param>
/// <param name="Level">The level of the player.</param>
/// <param name="Name">The name of the player.</param>
/// <param name="GroupId">The group id of the group character is in.</param>
/// <param name="Sex">The sex of the player.</param>
/// <param name="Class">The class of the player.</param>
/// <param name="MorphVNum">The morph of the player</param>
/// <param name="HeroLevel">The hero level of the player.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record PinitPlayerSubPacket
(
    [PacketIndex(0)]
    byte GroupPosition,
    [PacketIndex(1)]
    byte Level,
    [PacketIndex(2)]
    NameString? Name,
    [PacketIndex(3)]
    int? GroupId,
    [PacketIndex(4)]
    SexType Sex,
    [PacketIndex(5)]
    PlayerClass Class,
    [PacketIndex(6)]
    short MorphVNum,
    [PacketIndex(7)]
    byte? HeroLevel
) : IPacket;