//
//  RevivePacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Entities;

[PacketHeader("revive", PacketSource.Server)]
[GenerateSerializer(true)]
public record RevivePacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketIndex(2)]
    short TimeSpaceLives
) : IPacket;