//
//  GuriPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client.Movement;

/// <summary>
/// Dancing packet.
/// </summary>
/// <param name="Type">The type of the guri packet.</param>
/// <param name="Argument">The argument.</param>
/// <param name="EntityId">The id of the entity.</param>
/// <param name="Data">The data.</param>
/// <param name="Value">The value.</param>
[PacketHeader("guri", PacketSource.Client)]
[GenerateSerializer(true)]
public record GuriPacket
(
    [PacketIndex(0)]
    int Type,
    [PacketIndex(1, IsOptional = true)]
    int? Argument,
    [PacketIndex(2, IsOptional = true)]
    long? EntityId,
    [PacketIndex(3, IsOptional = true)]
    long? Data,
    [PacketIndex(4, IsOptional = true)]
    string? Value
) : IPacket;