//
//  CInfoPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Players;
using NosSmooth.PacketSerializer.Abstractions;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.Character;

/// <summary>
/// Information about the playing character.
/// </summary>
/// <remarks>
/// Sent on login and when changing map.
/// </remarks>
/// <param name="Name">The name of the character.</param>
/// <param name="Unknown">Unknown TODO</param>
/// <param name="GroupId">The id of the group the player is in, if any.</param>
/// <param name="FamilySubPacket">Information about family the player is in, if any.</param>
/// <param name="FamilyName">The name of the family the player is in, if any.</param>
/// <param name="CharacterId">The id of the character.</param>
/// <param name="Authority">The authority of the character.</param>
/// <param name="Sex">The sex of the character.</param>
/// <param name="HairStyle">The hair style of the character.</param>
/// <param name="HairColor">The hair color of the character.</param>
/// <param name="Class">The class of the character.</param>
/// <param name="Icon">Unknown TODO</param>
/// <param name="Compliment">Unknown TODO</param>
/// <param name="MorphVNum">The vnum of the morph (used for special cards, vehicles and such).</param>
/// <param name="IsInvisible">Whether the character is invisible.</param>
/// <param name="FamilyLevel">The level of the family, if any.</param>
/// <param name="MorphUpgrade">The upgrade of the morph (wings)</param>
/// <param name="ArenaWinner">Whether the character is an arena winner.</param>
[PacketHeader("c_info", PacketSource.Server)]
[GenerateSerializer(true)]
public record CInfoPacket
(
    [PacketIndex(0)]
    string Name,
    [PacketIndex(1)]
    string? Unknown,
    [PacketIndex(2)]
    short? GroupId,
    [PacketIndex(3, InnerSeparator = '.')]
    NullableWrapper<FamilySubPacket> FamilySubPacket,
    [PacketIndex(4)]
    NameString FamilyName,
    [PacketIndex(5)]
    long CharacterId,
    [PacketIndex(6)]
    AuthorityType Authority,
    [PacketIndex(7)]
    SexType Sex,
    [PacketIndex(8)]
    HairStyle HairStyle,
    [PacketIndex(9)]
    HairColor HairColor,
    [PacketIndex(10)]
    PlayerClass Class,
    [PacketIndex(11)]
    short Icon,
    [PacketIndex(12)]
    short Compliment,
    [PacketIndex(13)]
    short MorphVNum,
    [PacketIndex(14)]
    bool IsInvisible,
    [PacketIndex(15)]
    byte? FamilyLevel,
    [PacketIndex(16)]
    byte MorphUpgrade,
    [PacketIndex(17)]
    bool ArenaWinner
) : IPacket;
