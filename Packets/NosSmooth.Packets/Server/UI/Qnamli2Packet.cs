//
//  Qnamli2Packet.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.UI;

/// <summary>
/// Question and answer dialog that can have a location in the UI specified using <see cref="Type"/>.
/// Uses i18n message.
/// </summary>
/// <remarks>
/// To deny the dialog, just ignore it.
/// </remarks>
/// <param name="Type">The ui location.</param>
/// <param name="AcceptCommand">The command/packet to send to accept the dialog.</param>
/// <param name="MessageConst">The i18n message.</param>
/// <param name="ParametersCount">The count of parameters for the mssage.</param>
/// <param name="Parameters">The parameters for the message.</param>
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