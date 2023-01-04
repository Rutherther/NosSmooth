//
//  TwkPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.UI;

/// <summary>
/// Information about account, character, language.
/// </summary>
/// <param name="EntityType">The type of the character entity.</param>
/// <param name="EntityId">The id of the character.</param>
/// <param name="AccountName">The name of the logged in account.</param>
/// <param name="CharacterName">The name of the character.</param>
/// <param name="Salt">A salt, seems to always be shtmxpdlfeoqkr.</param>
/// <param name="ServerLanguage">The language of the server.</param>
/// <param name="ClientLanguage">The language of the client.</param>
[PacketHeader("twk", PacketSource.Server)]
[GenerateSerializer(true)]
public record TwkPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketIndex(2)]
    string AccountName,
    [PacketIndex(3)]
    NameString CharacterName,
    [PacketIndex(4)]
    string Salt,
    [PacketIndex(5, IsOptional = true)]
    string? ServerLanguage,
    [PacketIndex(6, IsOptional = true)]
    string? ClientLanguage
) : IPacket;