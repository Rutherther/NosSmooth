//
//  DlgiPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.UI;

[PacketHeader("dlgi", PacketSource.Server)]
[GenerateSerializer(true)]
public record DlgiPacket
(
    [PacketIndex(0)]
    string AcceptCommand,
    [PacketIndex(1)]
    string DenyCommand,
    [PacketIndex(2)]
    Game18NConstString MessageConst,
    [PacketListIndex(3, ListSeparator = ' ')]
    IReadOnlyList<string> Parameters
) : IPacket;