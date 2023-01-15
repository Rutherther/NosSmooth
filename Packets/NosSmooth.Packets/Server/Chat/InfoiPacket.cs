//
//  InfoiPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Chat;

/// <summary>
/// An information message.
/// </summary>
/// <param name="MessageConst">The message.</param>
/// <param name="Arguments">The arguments of the message constant.</param>
[PacketHeader("infoi", PacketSource.Server)]
[GenerateSerializer(true)]
public record InfoiPacket
(
    [PacketIndex(0)]
    Game18NConstString MessageConst,
    [PacketListIndex(1, ListSeparator = ' ')]
    IReadOnlyList<string> Arguments
) : IPacket;