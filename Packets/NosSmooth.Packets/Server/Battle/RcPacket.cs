//
//  RcPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Battle;

/// <summary>
/// A heal has been issued.
/// </summary>
/// <param name="EntityType">The type of the entity.</param>
/// <param name="EntityId">The id of the entity.</param>
/// <param name="Heal">The heal amount.</param>
/// <param name="Unknown">Unknown TODO.</param>
[PacketHeader("rc", PacketSource.Server)]
[GenerateSerializer(true)]
public record RcPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketIndex(2)]
    int Heal,
    [PacketIndex(3)]
    short Unknown
) : IPacket;