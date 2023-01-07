//
//  ScNSpSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Mates;

/// <summary>
/// A sub packet of <see cref="ScNPacket"/>
/// containing information about sp of the
/// partner.
/// </summary>
/// <param name="ItemVNum"></param>
/// <param name="AgilityPercentage"></param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record ScNSpSubPacket
(
    [PacketIndex(0)]
    long ItemVNum,
    [PacketIndex(1)]
    byte AgilityPercentage
);