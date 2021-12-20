//
//  PacketSerializerProvider.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using NosCore.Packets;

namespace NosSmooth.Core.Packets;

/// <summary>
/// Provides serializer for the server or client packets.
/// </summary>
public class PacketSerializerProvider
{
    private readonly List<Type> _clientPacketTypes;
    private readonly List<Type> _serverPacketTypes;

    private IPacketSerializer? _serverSerializer;
    private IPacketSerializer? _clientSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketSerializerProvider"/> class.
    /// </summary>
    /// <param name="clientPacketTypes">The types of client packets.</param>
    /// <param name="serverPacketTypes">The types of server packets.</param>
    public PacketSerializerProvider(List<Type> clientPacketTypes, List<Type> serverPacketTypes)
    {
        _clientPacketTypes = clientPacketTypes;
        _serverPacketTypes = serverPacketTypes;
    }

    /// <summary>
    /// Gets the server serializer.
    /// </summary>
    public IPacketSerializer ServerSerializer
    {
        get
        {
            if (_serverSerializer is null)
            {
                _serverSerializer =
                    new PacketSerializer(new Serializer(_serverPacketTypes), new Deserializer(_serverPacketTypes));
            }

            return _serverSerializer;
        }
    }

    /// <summary>
    /// Gets the client serializer.
    /// </summary>
    public IPacketSerializer ClientSerializer
    {
        get
        {
            if (_clientSerializer is null)
            {
                _clientSerializer =
                    new PacketSerializer(new Serializer(_clientPacketTypes), new Deserializer(_clientPacketTypes));
            }

            return _clientSerializer;
        }
    }
}