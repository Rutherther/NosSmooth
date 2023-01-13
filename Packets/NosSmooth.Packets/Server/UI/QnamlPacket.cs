//
//  QnamlPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.UI;

[PacketHeader("qnaml", PacketSource.Server)]
[GenerateSerializer(true)]
public record QnamlPacket
(
    [PacketIndex(0)]
    QnamlType Type,
    [PacketIndex(1)]
    string AcceptCommand,
    [PacketGreedyIndex(2)]
    string Message
) : IPacket;