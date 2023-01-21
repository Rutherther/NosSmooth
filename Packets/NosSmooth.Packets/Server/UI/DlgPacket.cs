//
//  DlgPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.UI;

/// <summary>
/// A dialog with normal string message.
/// </summary>
/// <remarks>
/// For a dialog with constant message, <see cref="DlgiPacket"/>.
/// </remarks>
/// <param name="AcceptCommand">The command/packet to send to accept the dialog.</param>
/// <param name="DenyCommand">The command/packet to send to deny the dialog.</param>
/// <param name="Message">The message of the dialog.</param>
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