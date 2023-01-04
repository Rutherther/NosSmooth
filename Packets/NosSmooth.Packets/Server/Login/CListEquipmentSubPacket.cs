//
//  CListEquipmentSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Login;

/// <summary>
/// Sub packet of <see cref="CListEquipmentSubPacket"/> present if the in packet
/// is for a player. Contains information about the player's
/// weapon.
/// /// </summary>
/// <param name="HatVNum">The VNum of the hat.</param>
/// <param name="ArmorVNum">The VNum of the armor.</param>
/// <param name="MainWeaponSkinOrWeaponVNum">The vnum of skin of the weapon or main weapon vnum.</param>
/// <param name="SecondaryWeaponVNum">The VNum of the secondary weapon.</param>
/// <param name="MaskVNum">The VNum of the mask.</param>
/// <param name="FairyVNum">The VNum of the fairy item.</param>
/// <param name="CostumeSuitVNum">The VNum of the costume suit.</param>
/// <param name="CostumeHatVNum">The VNum of the costume hat.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record CListEquipmentSubPacket
(
    [PacketIndex(0)]
    int? HatVNum,
    [PacketIndex(1)]
    int? ArmorVNum,
    [PacketIndex(2)]
    int? MainWeaponSkinOrWeaponVNum,
    [PacketIndex(3)]
    int? SecondaryWeaponVNum,
    [PacketIndex(4)]
    int? MaskVNum,
    [PacketIndex(5)]
    int? FairyVNum,
    [PacketIndex(6)]
    int? CostumeSuitVNum,
    [PacketIndex(7)]
    int? CostumeHatVNum
) : IPacket;