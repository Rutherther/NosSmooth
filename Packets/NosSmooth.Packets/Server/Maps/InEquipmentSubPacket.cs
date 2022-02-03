//
//  InEquipmentSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Maps;

/// <summary>
/// Sub packet of <see cref="InPacket"/> present if the in packet
/// is for a player. Contains information about the player's
/// weapon.
/// /// </summary>
/// <param name="HatVNum">The VNum of the hat.</param>
/// <param name="ArmorVNum">The VNum of the armor.</param>
/// <param name="MainWeaponVNum">The VNum of the main weapon.</param>
/// <param name="SecondaryWeaponVNum">The VNum of the secondary weapon.</param>
/// <param name="MaskVNum">The VNum of the mask.</param>
/// <param name="FairyVNum">The VNum of the fairy item.</param>
/// <param name="CostumeSuitVNum">The VNum of the costume suit.</param>
/// <param name="CostumeHatVNum">The VNum of the costume hat.</param>
/// <param name="WeaponSkin">The skin of the weapon.</param>
/// <param name="WingSkin">The skin of the wings.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record InEquipmentSubPacket
(
    [PacketIndex(0)]
    int? HatVNum,
    [PacketIndex(1)]
    int? ArmorVNum,
    [PacketIndex(2)]
    int? MainWeaponVNum,
    [PacketIndex(3)]
    int? SecondaryWeaponVNum,
    [PacketIndex(4)]
    int? MaskVNum,
    [PacketIndex(5)]
    int? FairyVNum,
    [PacketIndex(6)]
    int? CostumeSuitVNum,
    [PacketIndex(7)]
    int? CostumeHatVNum,
    [PacketIndex(8)]
    short? WeaponSkin,
    [PacketIndex(9)]
    short? WingSkin
) : IPacket;