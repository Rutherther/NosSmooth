//
//  PinitSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosCore.Shared.Enumerations;
using NosSmooth.Packets.Attributes;
using NosSmooth.Packets.Common;

namespace NosSmooth.Packets.Packets.Server.Groups;

/// <summary>
/// Sub packet of <see cref="PinitSubPacket"/> containing information
/// about one of the group members.
/// </summary>
/// <param name="EntityType">The type of the entity.</param>
/// <param name="EntityId">The id of the entity.</param>
/// <param name="GroupPosition">The position in the group.</param>
/// <param name="Level">The level of the entity.</param>
/// <param name="Name">The name of the entity.</param>
/// <param name="Unknown">Unknown.</param>
/// <param name="VNum">The VNum of the pet for pets.</param>
/// <param name="Race">The race of the entity.</param>
/// <param name="Morph">The morph of the entity.</param>
/// <param name="HeroLevel">The hero level of the entity.</param>
[GenerateSerializer]
[PacketHeader(null, PacketSource.Server)]
public record PinitSubPacket
(
    [PacketIndex(0)]
    VisualType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketIndex(2)]
    int GroupPosition,
    [PacketIndex(3)]
    byte Level,
    [PacketIndex(4)]
    NameString? Name,
    [PacketIndex(5)]
    int Unknown,
    [PacketIndex(6)]
    long VNum,
    [PacketIndex(7)]
    short Race,
    [PacketIndex(8)]
    short Morph,
    [PacketConditionalIndex(9, "EntityType", false, VisualType.Player)]
    byte? HeroLevel,
    [PacketConditionalIndex(10, "EntityType", false, VisualType.Player)]
    int? Unknown1,
    [PacketConditionalIndex(11, "EntityType", false, VisualType.Player)]
    int? Unknown2
) : IPacket;