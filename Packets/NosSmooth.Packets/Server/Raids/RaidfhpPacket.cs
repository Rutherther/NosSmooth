//
//  RaidfhpPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Raids;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Raids;

/// <summary>
/// Raid hp packet that is sent for raids using raidf packet.
/// Raids using raid packet contain hp, mp in raid packet itself.
/// </summary>
/// <param name="Type">The type of the raid packet (3 - player healths)</param>
/// <param name="HpSubPackets">Hp of the players.</param>
[PacketHeader("raidfhp", PacketSource.Server)]
[GenerateSerializer(true)]
public record RaidfhpPacket
(
    [PacketIndex(0)]
    RaidPacketType Type,
    [PacketListIndex(1, ListSeparator = ' ', InnerSeparator = '.')]
    IReadOnlyList<RaidfHpSubPacket> HpSubPackets
) : IPacket;