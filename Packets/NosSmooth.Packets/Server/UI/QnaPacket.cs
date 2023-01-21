//
//  QnaPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.UI;

/// <summary>
/// Question and answer dialog that uses string message.
/// </summary>
/// <remarks>
/// To deny the dialog, just ignore it.
/// This behaves as <see cref="QnamlPacket"/> with type <see cref="QnamlType.Dialog"/>.
/// </remarks>
/// <param name="AcceptCommand">The command/packet to send to accept the dialog.</param>
/// <param name="Message">The message of the dialog.</param>
[PacketHeader("qna", PacketSource.Server)]
[GenerateSerializer(true)]
public record QnaPacket
(
    [PacketIndex(0)]
    string AcceptCommand,
    [PacketGreedyIndex(1)]
    string Message
) : IPacket;