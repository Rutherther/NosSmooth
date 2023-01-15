//
//  MsgiPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Chat;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Chat;

/// <summary>
/// A constant i18n message received.
/// </summary>
/// <remarks>
/// Same as <see cref="Msgi2Packet"/>.
/// </remarks>
/// <param name="MessageType">The type of the message.</param>
/// <param name="MessageConst">The message.</param>
/// <param name="Arguments">The arguments of the message constant.</param>
[PacketHeader("msgi", PacketSource.Server)]
[GenerateSerializer(true)]
public record MsgiPacket
(
    [PacketIndex(0)]
    MessageType MessageType,
    [PacketIndex(1)]
    Game18NConstString MessageConst,
    [PacketListIndex(2, ListSeparator = ' ')]
    IReadOnlyList<string> Arguments
) : IPacket;