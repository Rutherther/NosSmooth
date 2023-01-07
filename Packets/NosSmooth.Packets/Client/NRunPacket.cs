//
//  NRunPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Entities;
using NosSmooth.Packets.Enums.NRun;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client;

/// <summary>
/// Npc run packet used for various operations.
/// </summary>
/// <param name="Type">The type of the nrun.</param>
/// <param name="SubType">The subtype, depends on the type. See NosSmooth.Packets.Enums.NRun.</param>
/// <param name="EntityType">The type of the entity (usually npc).</param>
/// <param name="EntityId">The id of the entity (npc).</param>
/// <param name="Confirmation">Unknown function. TODO.</param>
[PacketHeader("n_run", PacketSource.Client)]
[GenerateSerializer(true)]
public record NRunPacket
(
    [PacketIndex(0)]
    NRunType Type,
    [PacketIndex(1)]
    short SubType,
    [PacketIndex(2)]
    EntityType EntityType,
    [PacketIndex(3)]
    long EntityId,
    [PacketIndex(4, IsOptional = true)]
    byte? Confirmation
) : IPacket;