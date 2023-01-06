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