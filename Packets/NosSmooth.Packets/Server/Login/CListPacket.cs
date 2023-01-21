//
//  CListPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Players;
using NosSmooth.Packets.Server.Maps;
using NosSmooth.PacketSerializer.Abstractions;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.Login;

/// <summary>
/// Contains information about a character in character
/// slect upon login.
/// </summary>
/// <remarks>
/// Before clist, clist_start will be sent.
/// clist for each character will follow,
/// after that, clist_end will be sent.
/// </remarks>
/// <param name="Slot">The character slot the character is from.</param>
/// <param name="Name">The name of the character.</param>
/// <param name="Unknown">Unknown, seems to be always 0.</param>
/// <param name="Sex">The sex of the character.</param>
/// <param name="HairStyle">The hair style of the character.</param>
/// <param name="HairColor">The hair color of the character.</param>
/// <param name="Unknown1">Unknown, seems to be always 0.</param>
/// <param name="Class">The class of the character.</param>
/// <param name="Level">The level of the character.</param>
/// <param name="HeroLevel">The hero level of the character.</param>
/// <param name="EquipmentSubPacket">The equipment of the player.</param>
/// <param name="JobLevel">The job level of the character.</param>
/// <param name="Unknown2">Unknown, seems to be always 1.</param>
/// <param name="Unknown3">Unknown, seems to be always 1.</param>
/// <param name="PetsSubPacket">The pets of the character.</param>
/// <param name="HatDesign">The design of the hat.</param>
/// <param name="Unknown4">Unknown, seems to be always 0.</param>
[GenerateSerializer(true)]
[PacketHeader("clist", PacketSource.Server)]
public record CListPacket
(
    [PacketIndex(0)]
    byte Slot,
    [PacketIndex(1)]
    NameString Name,
    [PacketIndex(2)]
    byte Unknown,
    [PacketIndex(3)]
    SexType Sex,
    [PacketIndex(4)]
    HairStyle HairStyle,
    [PacketIndex(5)]
    HairColor HairColor,
    [PacketIndex(6)]
    byte Unknown1,
    [PacketIndex(7)]
    PlayerClass Class,
    [PacketIndex(8)]
    byte Level,
    [PacketIndex(9)]
    byte HeroLevel,
    [PacketIndex(10, InnerSeparator = '.')]
    CListEquipmentSubPacket EquipmentSubPacket,
    [PacketIndex(11, AllowMultipleSeparators = true)]
    byte JobLevel,
    [PacketIndex(12)]
    byte Unknown2,
    [PacketIndex(13)]
    byte Unknown3,
    [PacketListIndex(14, ListSeparator = '.', InnerSeparator = '.')]
    IReadOnlyList<OptionalWrapper<NullableWrapper<CListPetSubPacket>>> PetsSubPacket,
    [PacketIndex(15)]
    byte HatDesign,
    [PacketIndex(16)]
    byte Unknown4
) : IPacket;