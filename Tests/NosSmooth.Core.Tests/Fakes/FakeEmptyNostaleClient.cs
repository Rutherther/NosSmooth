//
//  FakeEmptyNostaleClient.cs
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

/// <inheritdoc />
public class FakeEmptyNostaleClient : INostaleClient
{
    /// <inheritdoc />
    public Task<Result> RunAsync(CancellationToken stopRequested = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result> SendPacketAsync(IPacket packet, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result> SendPacketAsync(string packetString, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result> ReceivePacketAsync(string packetString, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result> ReceivePacketAsync(IPacket packet, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Result> SendCommandAsync(ICommand command, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}