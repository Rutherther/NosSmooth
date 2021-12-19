using System;
using System.Threading.Tasks;
using NosCore.Packets;
using NosCore.Packets.Interfaces;
using Remora.Results;

namespace NosSmooth.Core.Packets;

public class PacketSerializer : IPacketSerializer
{
    private readonly Serializer _serializer;
    private readonly Deserializer _deserializer;

    public PacketSerializer(Serializer serializer, Deserializer deserializer)
    {
        _serializer = serializer;
        _deserializer = deserializer;
    }

    public Result<string> Serialize(IPacket packet)
    {
        try
        {
            return _serializer.Serialize(packet);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public Result<IPacket> Deserialize(string packetString)
    {
        try
        {
            return Result<IPacket>.FromSuccess(_deserializer.Deserialize(packetString));
        }
        catch (Exception e)
        {
            return e;
        }
    }
}