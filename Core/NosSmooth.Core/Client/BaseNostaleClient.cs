using System;
using System.Threading;
using System.Threading.Tasks;
using NosCore.Packets;
using NosCore.Packets.Interfaces;
using NosCore.Packets.ServerPackets.Event;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Packets;
using Remora.Results;

namespace NosSmooth.Core.Client;

public abstract class BaseNostaleClient : INostaleClient
{
    protected readonly CommandProcessor _commandProcessor;
    protected readonly IPacketSerializer _packetSerializer;

    protected BaseNostaleClient(CommandProcessor commandProcessor, IPacketSerializer packetSerializer)
    {
        _commandProcessor = commandProcessor;
        _packetSerializer = packetSerializer;
    }

    public abstract Task<Result> RunAsync(CancellationToken stopRequested = default);

    public virtual Task<Result> SendPacketAsync(IPacket packet, CancellationToken ct = default)
    {
        var serialized = _packetSerializer.Serialize(packet);

        return serialized.IsSuccess
            ? SendPacketAsync(serialized.Entity, ct) 
            : Task.FromResult(Result.FromError(serialized));
    }

    public abstract Task<Result> SendPacketAsync(string packetString, CancellationToken ct = default);
    public abstract Task<Result> ReceivePacketAsync(string packetString, CancellationToken ct = default);

    public virtual Task<Result> ReceivePacketAsync(IPacket packet, CancellationToken ct = default)
    {
        var serialized = _packetSerializer.Serialize(packet);

        return serialized.IsSuccess
            ? ReceivePacketAsync(serialized.Entity, ct) 
            : Task.FromResult(Result.FromError(serialized));
    }

    public Task<Result> SendCommandAsync(ICommand command, CancellationToken ct = default) =>
        _commandProcessor.ProcessCommand(command, ct);
}