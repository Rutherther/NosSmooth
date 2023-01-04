//
//  LbsPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Server.Login;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client.Login;

/// <summary>
/// Function unknown, sent
/// right after <see cref="GameStartPacket"/>.
/// </summary>
/// <remarks>
/// After <see cref="OkPacket"/> was received,
/// <see cref="GameStartPacket"/> and <see cref="LbsPacket"/>
/// should follow.
/// </remarks>
/// <param name="Unknown">Unknown, seems to be always 0.</param>
[PacketHeader("lbs", PacketSource.Client)]
[GenerateSerializer(true)]
public record LbsPacket
(
    [PacketIndex(0)]
    byte Unknown = 0
) : IPacket;