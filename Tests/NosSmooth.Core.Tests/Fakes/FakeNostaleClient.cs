//
//  FakeNostaleClient.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using NosSmooth.Packets;
using Remora.Results;

namespace NosSmooth.Core.Tests.Fakes;

/// <summary>
/// Fake NosTale client.
/// </summary>
public class FakeNostaleClient : INostaleClient
{
    private readonly Func<ICommand, CancellationToken, Result> _handleCommand;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeNostaleClient"/> class.
    /// </summary>
    /// <param name="handleCommand">The handler for <see cref="SendCommandAsync"/>.</param>
    public FakeNostaleClient(Func<ICommand, CancellationToken, Result> handleCommand)
    {
        _handleCommand = handleCommand;
    }

    /// <inheritdoc />
    public Task<Result> RunAsync(CancellationToken stopRequested = default)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result> SendPacketAsync(IPacket packet, CancellationToken ct = default)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result> SendPacketAsync(string packetString, CancellationToken ct = default)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result> ReceivePacketAsync(string packetString, CancellationToken ct = default)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result> ReceivePacketAsync(IPacket packet, CancellationToken ct = default)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result> SendCommandAsync(ICommand command, CancellationToken ct = default)
        => Task.FromResult(_handleCommand(command, ct));
}