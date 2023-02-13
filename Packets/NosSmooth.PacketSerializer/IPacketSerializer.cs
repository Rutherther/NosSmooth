//
//  IPacketSerializer.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.Packets;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using Remora.Results;

namespace NosSmooth.PacketSerializer;

/// <summary>
/// Serializer of packets.
/// </summary>
public interface IPacketSerializer
{
    /// <summary>
    /// Serializes the given object to string by appending to the packet string builder.
    /// </summary>
    /// <param name="obj">The packet to serialize.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result<string> Serialize(IPacket obj);

    /// <summary>
    /// Convert the data from the enumerator to the given type.
    /// </summary>
    /// <param name="packetString">The packet string to deserialize.</param>
    /// <param name="preferredSource">The preferred source to check first. If packet with the given header is not found there, other sources will be checked as well.</param>
    /// <returns>The parsed object or an error.</returns>
    public Result<IPacket> Deserialize(ReadOnlySpan<char> packetString, PacketSource preferredSource);
}