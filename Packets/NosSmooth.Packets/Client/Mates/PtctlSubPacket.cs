//
//  PtctlSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client.Mates;

/// <summary>
/// A sub packet of <see cref="PtctlPacket"/>.
/// </summary>
/// <param name="MateTransportId">The id of the mate.</param>
/// <param name="PositionX">The x coordinate to move to.</param>
/// <param name="PositionY">The y coordinate to move to.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record PtctlSubPacket
(
    [PacketIndex(0)]
    long MateTransportId,
    [PacketIndex(1)]
    short PositionX,
    [PacketIndex(2)]
    short PositionY
) : IPacket;