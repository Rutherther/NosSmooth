//
//  QnaPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.UI;

[PacketHeader("qna", PacketSource.Server)]
[GenerateSerializer(true)]
public record QnaPacket
(
    [PacketIndex(0)]
    string AcceptCommand,
    [PacketGreedyIndex(1)]
    string Message
) : IPacket;