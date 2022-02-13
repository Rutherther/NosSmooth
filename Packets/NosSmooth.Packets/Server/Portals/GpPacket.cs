//
//  GpPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Portals;

/// <summary>
/// Packet sent when joining a map with one portal that is on the map.
/// </summary>
/// <param name="X">The x coordinate of the portal.</param>
/// <param name="Y">The y coordinate of the portal.</param>
/// <param name="TargetMapId">The map the portal leads to.</param>
/// <param name="PortalType">The type of the portal.</param>
/// <param name="PortalId">The id of the portal entity.</param>
/// <param name="IsDisabled">Whether the portal is disabled/locked.</param>
[PacketHeader("gp", PacketSource.Server)]
[GenerateSerializer(true)]
public record GpPacket
(
    [PacketIndex(0)]
    short X,
    [PacketIndex(1)]
    short Y,
    [PacketIndex(2)]
    int TargetMapId,
    [PacketIndex(3)]
    PortalType? PortalType,
    [PacketIndex(4)]
    long PortalId,
    [PacketIndex(5)]
    bool IsDisabled
) : IPacket;