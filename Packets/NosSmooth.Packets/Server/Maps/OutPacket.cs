//
//  OutPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Maps;

/// <summary>
/// An entity has left the map.
/// </summary>
/// <param name="EntityType">The entity type.</param>
/// <param name="EntityId">The entity id.</param>
[PacketHeader("c_map", PacketSource.Server)]
[GenerateSerializer(true)]
public record OutPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId
) : IPacket;