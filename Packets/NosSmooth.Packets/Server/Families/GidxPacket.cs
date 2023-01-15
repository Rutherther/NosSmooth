//
//  GidxPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Entities;
using NosSmooth.Packets.Server.Character;
using NosSmooth.PacketSerializer.Abstractions;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.Families;

/// <summary>
/// Information about player's family.
/// </summary>
/// <param name="EntityType">The type of the entity (always a player).</param>
/// <param name="EntityId">The id of the player.</param>
/// <param name="FamilySubPacket">The sub packet with information about the family, or null, if not in family.</param>
/// <param name="FamilyName">The name of the family.</param>
/// <param name="FamilyCustomRank">The rank of the family.</param>
/// <param name="FamilyIcons">The icons of the family.</param>
[PacketHeader("gidx", PacketSource.Server)]
[GenerateSerializer(true)]
public record GidxPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketIndex(2, InnerSeparator = '.')]
    NullableWrapper<FamilySubPacket> FamilySubPacket,
    [PacketIndex(3)]
    NameString? FamilyName,
    [PacketIndex(4)]
    NameString? FamilyCustomRank,
    [PacketListIndex(5, ListSeparator = '|')]
    IReadOnlyList<bool> FamilyIcons
) : IPacket;