//
//  GameStartPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Server.Login;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client.Login;

/// <summary>
/// Start the game.
/// </summary>
/// <remarks>
/// Sent after <see cref="OkPacket"/> that was received
/// after sending <see cref="SelectPacket"/>.
///
/// After <see cref="GameStartPacket"/>, <see cref="LbsPacket"/>
/// should follow.
/// </remarks>
[GenerateSerializer(true)]
[PacketHeader("game_start", PacketSource.Client)]
public record GameStartPacket() : IPacket;