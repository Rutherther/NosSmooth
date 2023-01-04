//
//  BnPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.UI;

/// <summary>
/// A broadcast message.
/// </summary>
/// <remarks>
/// In the client this is shown on bottom left
/// under the chat.
/// </remarks>
/// <param name="BnNumber">The number of the message.</param>
/// <param name="Message">The message.</param>
[PacketHeader("bn", PacketSource.Server)]
[GenerateSerializer(true)]
public record BnPacket
(
    [PacketIndex(0)]
    byte BnNumber,
    [PacketIndex(1)]
    NameString Message
) : IPacket;