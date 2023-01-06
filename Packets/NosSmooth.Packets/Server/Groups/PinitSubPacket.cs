//
//  PinitSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.Packets.Enums.Mates;
using NosSmooth.Packets.Enums.Players;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.Groups;

/// <summary>
/// Sub packet of <see cref="PinitSubPacket"/> containing information
/// about one of the group members.
/// </summary>
/// <param name="EntityType">The type of the entity.</param>
/// <param name="EntityId">The id of the entity.</param>
/// <param name="MateSubPacket">Present for pets and partners.</param>
/// <param name="PlayerSubPacket">Present for players.</param>
[GenerateSerializer(true)]
[PacketHeader(null, PacketSource.Server)]
public record PinitSubPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketConditionalIndex(2, "EntityType", false, EntityType.Npc, InnerSeparator = '|')]
    PinitMateSubPacket? MateSubPacket,
    [PacketConditionalIndex(3, "EntityType", false, EntityType.Player, InnerSeparator = '|')]
    PinitPlayerSubPacket? PlayerSubPacket
) : IPacket;