//
//  RsfpPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Character;

/// <summary>
/// Unknown. Sent upon world initialization.
/// </summary>
/// <param name="MapX">Unknown TODO.</param>
/// <param name="MapY">Unknown TODO.</param>
[PacketHeader("rsfp", PacketSource.Server)]
[GenerateSerializer(true)]
public record RsfpPacket
(
    [PacketIndex(0)]
    short? MapX,
    [PacketIndex(1)]
    short? MapY
) : IPacket;