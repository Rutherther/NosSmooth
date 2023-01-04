//
//  FdPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Character;

/// <summary>
/// Reputation and dignity packet.
/// </summary>
/// <param name="Reputation">The character's reputation.</param>
/// <param name="ReputationIcon">The character's reputation icon.</param>
/// <param name="Dignity">The character's dignity.</param>
/// <param name="DignityIcon">The character's dignity icon.</param>
[PacketHeader("fd", PacketSource.Server)]
[GenerateSerializer(true)]
public record FdPacket
(
    [PacketIndex(0)]
    long Reputation,
    [PacketIndex(1)]
    short ReputationIcon,
    [PacketIndex(2)]
    int Dignity,
    [PacketIndex(3)]
    short DignityIcon
) : IPacket;