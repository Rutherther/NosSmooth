//
//  PacketEventArgs.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Core.Packets;

/// <summary>
/// Arguments for <see cref="IPacketResponder{TPacket}"/>
/// </summary>
/// <param name="Source">The source of the packet.</param>
/// <param name="Packet">The deserialized packet.</param>
/// <param name="PacketString">The packet string.</param>
public record PacketEventArgs<TPacket>(PacketSource Source, TPacket Packet, string PacketString);