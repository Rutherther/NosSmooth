//
//  RestPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.UI;

/// <summary>
/// The given entity sitting state has been changed.
/// </summary>
/// <param name="EntityType">The type of the entity.</param>
/// <param name="EntityId">The id of the entity.</param>
/// <param name="IsSitting">Whether the entity is sitting.</param>
[PacketHeader("rest", PacketSource.Server)]
[GenerateSerializer(true)]
public record RestPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketIndex(2)]
    bool IsSitting
) : IPacket;