//
//  BfEPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Buffs;

/// <summary>
/// A const buff effect.
/// </summary>
/// <param name="EntityType">The type of the entity.</param>
/// <param name="EntityId">The id of the entity.</param>
/// <param name="CardId">The buff card id.</param>
/// <param name="Time">The duration of the buff.</param>
[PacketHeader("bf_e", PacketSource.Server)]
[GenerateSerializer(true)]
public record BfEPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketIndex(2)]
    short CardId,
    [PacketIndex(3)]
    int Time
) : IPacket;