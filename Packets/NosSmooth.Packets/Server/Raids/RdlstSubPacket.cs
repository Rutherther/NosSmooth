//
//  RdlstSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Players;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.Raids;

/// <summary>
/// A sub packet of <see cref="RdlstPacket"/>.
/// Information about a member of the raid.
/// </summary>
/// <param name="Level">The level of the player.</param>
/// <param name="MorphVNum">The morph vnum of the player.</param>
/// <param name="Class">The class of the player.</param>
/// <param name="Deaths">The current number of deaths in the raid.</param>
/// <param name="Name">The name of the player.</param>
/// <param name="Sex">The sex of the player.</param>
/// <param name="Id">The id of the player entity.</param>
/// <param name="HeroLevel">The hero level of the player.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record RdlstSubPacket
(
    [PacketIndex(0)]
    byte Level,
    [PacketIndex(1)]
    int? MorphVNum,
    [PacketIndex(2)]
    PlayerClass Class,
    [PacketIndex(3)]
    byte Deaths,
    [PacketIndex(4)]
    NameString Name,
    [PacketIndex(5)]
    SexType Sex,
    [PacketIndex(6)]
    long Id,
    [PacketIndex(7)]
    byte? HeroLevel
);