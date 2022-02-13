//
//  WalkPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client.Movement;

/// <summary>
/// Packet sent when moving.
/// </summary>
/// <remarks>
/// This packet is sent every x seconds
/// when moving. The packet is sent before the
/// player has actually reached the position.
///
/// The server should respond with "at" packet.
/// </remarks>
/// <param name="PositionX">The position the character is walking to.</param>
/// <param name="PositionY"></param>
/// <param name="CheckSum">The check sum. Has value: (walkPacket.PositionX + walkPacket.PositionY) % 3 % 2. </param>
/// <param name="Speed">The movement speed.</param>
[PacketHeader("walk", PacketSource.Client)]
[GenerateSerializer(true)]
public record WalkPacket
(
    [PacketIndex(0)]
    ushort PositionX,
    [PacketIndex(1)]
    ushort PositionY,
    [PacketIndex(2)]
    byte CheckSum,
    [PacketIndex(3)]
    short Speed
) : IPacket;