//
//  SayPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Attributes;

namespace NosSmooth.Packets.Packets.Client.Chat;

[PacketHeader("say", PacketSource.Client)]
[GenerateSerializer(true)]
public record SayPacket
(
    [PacketGreedyIndex(0)]
    string Message
) : IPacket;