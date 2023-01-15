//
//  SuctlPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client.Mates;

/// <summary>
/// Attack with the given mate.
/// </summary>
/// <param name="CastId">The id of the skill cast.</param>
/// <param name="EntityType">The type of the mate entity.</param>
/// <param name="MateTransportId">The id of the mate.</param>
/// <param name="TargetType">The entity type of the target.</param>
/// <param name="TargetId">The id of the target.</param>
[PacketHeader("suctl", PacketSource.Server)]
[GenerateSerializer(true)]
public record SuctlPacket
(
    [PacketIndex(0)]
    int CastId,
    [PacketIndex(1)]
    EntityType EntityType,
    [PacketIndex(2)]
    int MateTransportId,
    [PacketIndex(3)]
    EntityType TargetType,
    [PacketIndex(4)]
    long TargetId
) : IPacket;