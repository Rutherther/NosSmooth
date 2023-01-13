//
//  Qnamli2Packet.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.UI;

[PacketHeader("qnamli2", PacketSource.Server)]
[GenerateSerializer(true)]
public record Qnamli2Packet
(
    [PacketIndex(0)]
    QnamlType Type,
    [PacketIndex(1)]
    string AcceptCommand,
    [PacketIndex(2)]
    Game18NConstString MessageConst,
    [PacketIndex(3)]
    short ParametersCount,
    [PacketListIndex(4, ListSeparator = ' ')]
    IReadOnlyList<string> Parameters
) : IPacket;