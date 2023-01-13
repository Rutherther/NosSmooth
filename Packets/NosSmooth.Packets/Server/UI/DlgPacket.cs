//
//  DlgPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.UI;

[PacketHeader("dlg", PacketSource.Server)]
[GenerateSerializer(true)]
public record DlgPacket
(
    [PacketIndex(0)]
    string AcceptCommand,
    [PacketIndex(1)]
    string DenyCommand,
    [PacketGreedyIndex(2)]
    string Message
) : IPacket;