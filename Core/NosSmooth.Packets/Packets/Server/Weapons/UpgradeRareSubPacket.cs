//
//  UpgradeRareSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Attributes;

namespace NosSmooth.Packets.Packets.Server.Weapons;

/// <summary>
/// Upgrade and rare of a weapon or an armor.
/// </summary>
/// <param name="Upgrade">The upgrade of the weapon.</param>
/// <param name="Rare">The rare .</param>
[PacketHeader(null, PacketSource.Server)]
public record UpgradeRareSubPacket
(
    byte Upgrade,
    sbyte Rare
) : IPacket;