//
//  PinitPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Groups;

/// <summary>
/// Sent for initialization of pets and groups.
/// </summary>
/// <remarks>
/// Contains pet and group information.
/// </remarks>
/// <param name="SubPacketsCount">The number of sub packets.</param>
/// <param name="PinitSubPackets">The members of the group. (including pet and partner, if any)</param>
[GenerateSerializer(true)]
[PacketHeader("pinit", PacketSource.Server)]
public record PinitPacket
(
    [PacketIndex(0)]
    byte SubPacketsCount,
    [PacketListIndex(1, ListSeparator = ' ', InnerSeparator = '|')]
    IReadOnlyList<PinitSubPacket> PinitSubPackets
) : IPacket;