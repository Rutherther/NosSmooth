//
//  GpPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Portals;

[PacketHeader("gp", PacketSource.Server)]
[GenerateSerializer(true)]
public record GpPacket
(
    [PacketIndex(0)]
    short X,
    [PacketIndex(1)]
    short Y,
    [PacketIndex(2)]
    short TargetMapId,
    [PacketIndex(3)]
    PortalType? PortalType,
    [PacketIndex(4)]
    int PortalId,
    [PacketIndex(5)]
    bool IsDisabled
) : IPacket;