//
//  PacketSerializer.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using NosCore.Packets;
using NosCore.Packets.Interfaces;
using NosSmooth.Core.Packets.Converters;
using Remora.Results;

namespace NosSmooth.Core.Packets;

/// <inheritdoc />
public class PacketSerializer : IPacketSerializer
{
    private readonly Serializer _serializer;
    private readonly Deserializer _deserializer;
    private readonly IEnumerable<ISpecificPacketSerializer> _specificPacketSerializers;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketSerializer"/> class.
    /// </summary>
    /// <param name="serializer">The NosCore serializer.</param>
    /// <param name="deserializer">The NosCore deserializer.</param>
    /// <param name="specificPacketSerializers">The specific packet serializers.</param>
    public PacketSerializer
    (
        Serializer serializer,
        Deserializer deserializer,
        IEnumerable<ISpecificPacketSerializer> specificPacketSerializers
    )
    {
        _serializer = serializer;
        _deserializer = deserializer;
        _specificPacketSerializers = specificPacketSerializers;
    }

    /// <inheritdoc />
    public Result<string> Serialize(IPacket packet)
    {
        try
        {
            foreach (var specificPacketSerializer in _specificPacketSerializers)
            {
                if (specificPacketSerializer.Serializer && specificPacketSerializer.ShouldHandle(packet))
                {
                    return specificPacketSerializer.Serialize(packet);
                }
            }

            return _serializer.Serialize(packet);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <inheritdoc />
    public Result<IPacket> Deserialize(string packetString)
    {
        try
        {
            foreach (var specificPacketSerializer in _specificPacketSerializers)
            {
                if (specificPacketSerializer.Deserializer && specificPacketSerializer.ShouldHandle(packetString))
                {
                    return specificPacketSerializer.Deserialize(packetString);
                }
            }

            return Result<IPacket>.FromSuccess(_deserializer.Deserialize(packetString));
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <inheritdoc />
    public Serializer Serializer => _serializer;

    /// <inheritdoc />
    public Deserializer Deserializer => _deserializer;
}