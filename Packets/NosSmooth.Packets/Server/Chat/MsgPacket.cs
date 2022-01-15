//
//  MsgPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Chat;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Chat;

/// <summary>
/// Contains message that was sent in chat, shouted etc.
/// </summary>
/// <param name="Type">The type of the message.</param>
/// <param name="Message">The message text.</param>
[PacketHeader("msg", PacketSource.Server)]
[GenerateSerializer(true)]
public record MsgPacket
(
    [PacketIndex(0)]
    MessageType Type,
    [PacketGreedyIndex(1)]
    string Message
) : IPacket;