//
//  QnamlPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.UI;

/// <summary>
/// Question and answer dialog that can have a location in the UI specified using <see cref="Type"/>.
/// Uses string message.
/// </summary>
/// <remarks>
/// To deny the dialog, just ignore it.
/// </remarks>
/// <param name="Type">The ui location.</param>
/// <param name="AcceptCommand">The command/packet to send to accept the dialog.</param>
/// <param name="Message">The message of the dialog.</param>
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