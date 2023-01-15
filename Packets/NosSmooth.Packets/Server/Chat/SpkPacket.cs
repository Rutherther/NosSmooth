//
//  SpkPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Chat;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.Chat;

/// <summary>
/// An entity speak.
/// </summary>
/// <param name="EntityType">The type of the entity that spoke.</param>
/// <param name="EntityId">The id of the entity that spoke.</param>
/// <param name="Type">The type of the speak/message.</param>
/// <param name="Name">The name of the entity.</param>
/// <param name="Message">The spoken message.</param>
[PacketHeader("spk", PacketSource.Server)]
[GenerateSerializer(true)]
public record SpkPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketIndex(2)]
    SpeakType Type,
    [PacketIndex(3)]
    NameString Name,
    [PacketGreedyIndex(4, IsOptional = true)]
    string? Message
) : IPacket;