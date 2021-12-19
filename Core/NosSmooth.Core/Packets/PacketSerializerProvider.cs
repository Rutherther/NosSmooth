using System;
using System.Collections.Generic;
using NosCore.Packets;

namespace NosSmooth.Core.Packets;

public class PacketSerializerProvider
{
    private readonly List<Type> _clientPacketTypes;
    private readonly List<Type> _serverPacketTypes;

    private IPacketSerializer? _serverSerializer;
    private IPacketSerializer? _clientSerializer;

    public PacketSerializerProvider(List<Type> clientPacketTypes, List<Type> serverPacketTypes)
    {
        _clientPacketTypes = clientPacketTypes;
        _serverPacketTypes = serverPacketTypes;
    }

    public IPacketSerializer GetServerSerializer()
    {
        if (_serverSerializer is null)
        {
            _serverSerializer =
                new PacketSerializer(new Serializer(_serverPacketTypes), new Deserializer(_serverPacketTypes));
        }

        return _serverSerializer;
    }

    public IPacketSerializer GetClientSerializer()
    {
        if (_clientSerializer is null)
        {
            _clientSerializer =
                new PacketSerializer(new Serializer(_clientPacketTypes), new Deserializer(_clientPacketTypes));
        }

        return _clientSerializer;
    }
}