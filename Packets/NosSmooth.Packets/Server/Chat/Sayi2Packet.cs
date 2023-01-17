//
//  Sayi2Packet.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Chat;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Chat;

/// <summary>
/// A message from an entity in chat.
/// </summary>
/// <param name="EntityType">The type of the entity that spoke.</param>
/// <param name="EntityId">The id of the entity that spoke.</param>
/// <param name="Color">The say/message color.</param>
/// <param name="MessageConst">The message constant.</param>
/// <param name="Arguments">The arguments of the message.</param>
[PacketHeader("sayi2", PacketSource.Server)]
[GenerateSerializer(true)]
public record Sayi2Packet
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketIndex(2)]
    SayColor Color,
    [PacketIndex(3)]
    Game18NConstString MessageConst,
    [PacketIndex(4)]
    short ParametersCount,
    [PacketListIndex(5, ListSeparator = ' ')]
    IReadOnlyList<string> Parameters
) : IPacket;