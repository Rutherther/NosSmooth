//
//  IPacketSerializer.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosCore.Packets;
using NosCore.Packets.Interfaces;
using Remora.Results;

namespace NosSmooth.Core.Packets;

/// <summary>
/// Represents serialiazer and deserializer of <see cref="IPacket"/>.
/// </summary>
public interface IPacketSerializer
{
    /// <summary>
    /// Serializes the given <see cref="IPacket"/> into string.
    /// </summary>
    /// <param name="packet">The packet to serialize.</param>
    /// <returns>The serialized packet.</returns>
    public Result<string> Serialize(IPacket packet);

    /// <summary>
    /// Deserializes the given string into <see cref="IPacket"/>.
    /// </summary>
    /// <param name="packetString">The packet to deserialize.</param>
    /// <returns>The deserialized packet.</returns>
    public Result<IPacket> Deserialize(string packetString);

    /// <summary>
    /// Gets the inner serializer from NosCore.
    /// </summary>
    [Obsolete("May be removed anytime.")]
    public Serializer Serializer { get; }

    /// <summary>
    /// Gets the inner deserializer from NosCore.
    /// </summary>
    [Obsolete("May be removed anytime.")]
    public Deserializer Deserializer { get; }
}