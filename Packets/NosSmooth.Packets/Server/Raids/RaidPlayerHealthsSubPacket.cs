//
//  RaidPlayerHealthsSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Raids;

/// <summary>
/// A sub packet of <see cref="RaidPacket"/>
/// present for PlayerHealths. Contains
/// information about player healths.
/// </summary>
/// <param name="PlayerId">The id of the player.</param>
/// <param name="HpPercentage">The hp percentage of the player.</param>
/// <param name="MpPercentage">The mp percentage of the player.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record RaidPlayerHealthsSubPacket
(
    [PacketIndex(0)]
    long PlayerId,
    [PacketIndex(1)]
    byte HpPercentage,
    [PacketIndex(2)]
    byte MpPercentage
) : IPacket;