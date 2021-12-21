//
//  PacketEventArgs.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosCore.Packets.Interfaces;

namespace NosSmooth.Core.Packets;

/// <summary>
/// Arguments for <see cref="IPacketResponder{TPacket}"/>
/// </summary>
/// <param name="Type">The type of the packet.</param>
/// <param name="Packet">The deserialized packet.</param>
/// <param name="PacketString">The packet string.</param>
public record PacketEventArgs<TPacket>(PacketType Type, TPacket Packet, string PacketString);