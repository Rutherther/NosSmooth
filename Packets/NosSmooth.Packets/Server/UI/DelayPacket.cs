//
//  DelayPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.UI;

/// <summary>
/// The client should wait before doing the given operation,
/// dancing in the process. The character may not move, sit, etc.
/// In case the character moves, sits or does something else,
/// the operation will be cancelled.
/// </summary>
/// <param name="Delay">The time to wait.</param>
/// <param name="Type">The type of the delay packet.</param>
/// <param name="Argument">The argument to send after waited.</param>
[PacketHeader("delay", PacketSource.Server)]
[GenerateSerializer(true)]
public record DelayPacket
(
    [PacketIndex(0)]
    int Delay,
    [PacketIndex(1)]
    int Type,
    [PacketIndex(2)]
    string Argument
) : IPacket;