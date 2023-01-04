//
//  TitinfoPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Character;

/// <summary>
/// Information about titles of the character.
/// </summary>
/// <param name="EntityType">The type of the entity, seems to be always a player.</param>
/// <param name="CharacterId">The id of the character.</param>
/// <param name="UnknownTitleVNum1">Unknown, seems to be vnum of title, but which one?</param>
/// <param name="UnknownTitleVnum2">Unknown, seems to be vnum of title, but which one?</param>
[PacketHeader("titinfo", PacketSource.Server)]
[GenerateSerializer(true)]
public record TitinfoPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long CharacterId,
    [PacketIndex(2)]
    long UnknownTitleVNum1,
    [PacketIndex(3)]
    long UnknownTitleVnum2
) : IPacket;