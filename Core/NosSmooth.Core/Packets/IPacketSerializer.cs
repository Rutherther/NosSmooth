using System.Threading.Tasks;
using NosCore.Packets.Interfaces;
using Remora.Results;

namespace NosSmooth.Core.Packets;

public interface IPacketSerializer
{
    public Result<string> Serialize(IPacket packet);

    public Result<IPacket> Deserialize(string packetString);
}