//
//  BaseNostaleClient.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Commands;
using NosSmooth.Packets;
using NosSmooth.PacketSerializer;
using Remora.Results;

namespace NosSmooth.Core.Client;

/// <summary>
/// Represents base class of <see cref="INostaleClient"/>.
/// </summary>
/// <remarks>
/// This class serializes packets and processes commands.
/// </remarks>
public abstract class BaseNostaleClient : INostaleClient
{
    private readonly CommandProcessor _commandProcessor;
    private readonly IPacketSerializer _packetSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseNostaleClient"/> class.
    /// </summary>
    /// <param name="commandProcessor">The command processor.</param>
    /// <param name="packetSerializer">The packet serializer.</param>
    protected BaseNostaleClient
    (
        CommandProcessor commandProcessor,
        IPacketSerializer packetSerializer
    )
    {
        _commandProcessor = commandProcessor;
        _packetSerializer = packetSerializer;
    }

    /// <inheritdoc />
    public abstract Task<Result> RunAsync(CancellationToken stopRequested = default);

    /// <inheritdoc />
    public virtual Task<Result> SendPacketAsync(IPacket packet, CancellationToken ct = default)
    {
        var serialized = _packetSerializer.Serialize(packet);

        return serialized.IsSuccess
            ? SendPacketAsync(serialized.Entity, ct)
            : Task.FromResult(Result.FromError(serialized));
    }

    /// <inheritdoc />
    public abstract Task<Result> SendPacketAsync(string packetString, CancellationToken ct = default);

    /// <inheritdoc />
    public abstract Task<Result> ReceivePacketAsync(string packetString, CancellationToken ct = default);

    /// <inheritdoc />
    public virtual Task<Result> ReceivePacketAsync(IPacket packet, CancellationToken ct = default)
    {
        var serialized = _packetSerializer.Serialize(packet);

        return serialized.IsSuccess
            ? ReceivePacketAsync(serialized.Entity, ct)
            : Task.FromResult(Result.FromError(serialized));
    }

    /// <inheritdoc />
    public Task<Result> SendCommandAsync(ICommand command, CancellationToken ct = default)
        => _commandProcessor.ProcessCommand(this, command, ct);
}
