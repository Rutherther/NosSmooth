//
//  SayPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Attributes;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Chat;

namespace NosSmooth.Packets.Packets.Server.Chat;

/// <summary>
/// Sent when someone says something.
/// </summary>
/// <param name="EntityType">The entity type that said something.</param>
/// <param name="EntityId">The id of the entity that said something.</param>
/// <param name="Color">The color of the message in chat.</param>
/// <param name="Message">The message received.</param>
[PacketHeader("say", PacketSource.Server)]
[GenerateSerializer(true)]
public record SayPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketIndex(2)]
    SayColor Color,
    [PacketGreedyIndex(3)]
    string Message
) : IPacket;