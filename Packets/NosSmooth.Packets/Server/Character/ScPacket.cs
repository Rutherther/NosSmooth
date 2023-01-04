//
//  ScPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Character;

/// <summary>
/// Information about character's weapons, armor, resistance.
/// </summary>
/// <param name="ClassMinusOne">The class of the character minus one.</param>
/// <param name="MainWeaponSubPacket">The information about character's main weapon.</param>
/// <param name="IsArcher">Whether the character is an archer.</param>
/// <param name="SecondaryWeaponSubPacket">The information about character's secondary weapon.</param>
/// <param name="ArmorSubPacket">The information about character's armor.</param>
/// <param name="ResistanceSubPacket">The character's resistance</param>
[PacketHeader("sc", PacketSource.Server)]
[GenerateSerializer(true)]
public record ScPacket
(
    [PacketIndex(0)]
    byte ClassMinusOne,
    [PacketIndex(1, InnerSeparator = ' ')]
    ScWeaponSubPacket MainWeaponSubPacket,
    [PacketIndex(2)]
    byte IsArcher,
    [PacketIndex(3, InnerSeparator = ' ')]
    ScWeaponSubPacket SecondaryWeaponSubPacket,
    [PacketIndex(4)]
    ScArmorSubPacket ArmorSubPacket,
    [PacketIndex(5)]
    ResistanceSubPacket ResistanceSubPacket
) : IPacket;