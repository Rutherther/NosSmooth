//
//  SelectPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Server.Login;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client.Login;

/// <summary>
/// Select character on the given slot from <see cref="CListPacket"/>.
/// </summary>
/// <remarks>
/// <see cref="OkPacket"/> should be sent from the server
/// after select.
///
/// Then the client sends <see cref="GameStartPacket"/>
/// and <see cref="LbsPacket"/>.
/// </remarks>
/// <param name="Slot">The slot of the character from clist.</param>
[GenerateSerializer(true)]
[PacketHeader("select", PacketSource.Client)]
public record SelectPacket
(
    [PacketIndex(0)]
    byte Slot
) : IPacket;