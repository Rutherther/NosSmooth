//
//  RdlstfPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Raids;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Raids;

/// <summary>
/// A packet containing information about raid members.
/// </summary>
/// <param name="MinimumLevel">The minimum needed level for the raid treasure.</param>
/// <param name="MaximumLevel">The maximum needed level for the raid treasure.</param>
/// <param name="Unknown">Unknown TODO.</param>
/// <param name="RaidType">The raid, <see cref="RaidType"/>.</param>
/// <param name="Players">Information about members in the raid.</param>
[PacketHeader("rdlstf", PacketSource.Server)]
[GenerateSerializer(true)]
public record RdlstfPacket
(
    [PacketIndex(0)]
    short MinimumLevel,
    [PacketIndex(1)]
    short MaximumLevel,
    [PacketIndex(2)]
    short Unknown,
    [PacketIndex(3)]
    RaidType RaidType,
    [PacketListIndex(4, ListSeparator = ' ', InnerSeparator = '.')]
    IReadOnlyList<RdlstSubPacket> Players
) : IPacket;