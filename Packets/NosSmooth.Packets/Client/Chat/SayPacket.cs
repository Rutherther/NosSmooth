//
//  SayPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client.Chat;

/// <summary>
/// Sends a message to the map chat.
/// </summary>
/// <param name="Message">The message to send.</param>
[PacketHeader("say", PacketSource.Client)]
[GenerateSerializer(true)]
public record SayPacket
(
    [PacketGreedyIndex(0)]
    string Message
) : IPacket;