//
//  ScNEquipmentSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Mates;

/// <summary>
/// A sub packet of <see cref="ScNPacket"/>
/// containing information about partner's
/// equipped item.
/// </summary>
/// <param name="ItemVNum"></param>
/// <param name="ItemRare"></param>
/// <param name="ItemUpgrade"></param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record ScNEquipmentSubPacket
(
    [PacketIndex(0)]
    int? ItemVNum,
    [PacketIndex(1, IsOptional = true)]
    sbyte? ItemRare,
    [PacketIndex(2, IsOptional = true)]
    byte? ItemUpgrade
) : IPacket;