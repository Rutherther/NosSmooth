//
//  ScArmorSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Character;

/// <summary>
/// A sub packet for <see cref="ScPacket"/>
/// containing information about armor.
/// </summary>
/// <param name="ArmorUpgrade">The upgrade of the armor</param>
/// <param name="MeleeDefence">The melee defence of the armor.</param>
/// <param name="MeleeDefenceDodgeRate">The melee dodge rate of the armor.</param>
/// <param name="RangeDefence">The ranged defence of the armor.</param>
/// <param name="RangeDefenceDodgeRate">The ranged dodge rate of the armor.</param>
/// <param name="MagicalDefence">The magical defence of the armor.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record ScArmorSubPacket
(
    [PacketIndex(0)]
    byte ArmorUpgrade,
    [PacketIndex(1)]
    int MeleeDefence,
    [PacketIndex(2)]
    int MeleeDefenceDodgeRate,
    [PacketIndex(3)]
    int RangeDefence,
    [PacketIndex(4)]
    int RangeDefenceDodgeRate,
    [PacketIndex(5)]
    int MagicalDefence
) : IPacket;