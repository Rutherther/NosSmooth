//
//  RbossPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Raids;

/// <summary>
/// Raid boss information.
/// </summary>
/// <remarks>
/// EntityType and EntityId will be null in case of no boss.
/// </remarks>
/// <param name="EntityType">The boss entity type.</param>
/// <param name="EntityId">The boss entity id.</param>
/// <param name="MaxHp">The max hp of the boss.</param>
/// <param name="VNum">The vnum of the boss entity.</param>
[PacketHeader("rboss", PacketSource.Server)]
[GenerateSerializer(true)]
public record RbossPacket
(
    [PacketIndex(0)]
    EntityType? EntityType,
    [PacketIndex(1)]
    long? EntityId,
    [PacketIndex(2)]
    int Hp,
    [PacketIndex(3)]
    int MaxHp,
    [PacketIndex(4)]
    int VNum
) : IPacket;