//
//  CMapPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Maps;

/// <summary>
/// Packet sent when map is changed.
/// </summary>
/// <param name="Type">The type of the map.</param>
/// <param name="Id">The id of the map.</param>
/// <param name="IsBaseType">Whether the map is a base map.</param>
[PacketHeader("c_map", PacketSource.Server)]
[GenerateSerializer(true)]
public record CMapPacket
(
    [PacketIndex(0)]
    byte Type,
    [PacketIndex(1)]
    short Id,
    [PacketIndex(2)]
    bool IsBaseType
) : IPacket;