//
//  ScWeaponSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Character;

/// <summary>
/// A sub packet of <see cref="ScPacket"/>
/// containing information about character's weapon.
/// </summary>
/// <param name="WeaponUpgrade">The upgrade of the weapon.</param>
/// <param name="MinHit">The minimal hit amount.</param>
/// <param name="MaxHit">The maximal hit amount.</param>
/// <param name="HitRate">The hit rate.</param>
/// <param name="CriticalHitRate">The critical hit rate.</param>
/// <param name="CriticalHitMultiplier">The critical hit multiplier.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record ScWeaponSubPacket
(
    [PacketIndex(0)]
    byte WeaponUpgrade,
    [PacketIndex(1)]
    int MinHit,
    [PacketIndex(2)]
    int MaxHit,
    [PacketIndex(3)]
    int HitRate,
    [PacketIndex(4)]
    int CriticalHitRate,
    [PacketIndex(5)]
    int CriticalHitMultiplier
) : IPacket;