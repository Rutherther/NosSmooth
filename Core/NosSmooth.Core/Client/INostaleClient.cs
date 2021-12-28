//
//  INostaleClient.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using NosCore.Packets.Interfaces;
using NosSmooth.Core.Commands;
using Remora.Results;

namespace NosSmooth.Core.Client;

/// <summary>
/// Class representing nostale client that may send and receive packets as well as process commands.
/// </summary>
public interface INostaleClient
{
    /// <summary>
    /// Starts the client.
    /// </summary>
    /// <param name="stopRequested">A cancellation token for stopping the client.</param>
    /// <returns>The result that may or may not have succeeded.</returns>
    public Task<Result> RunAsync(CancellationToken stopRequested = default);

    /// <summary>
    /// Sends the given packet to the server.
    /// </summary>
    /// <param name="packet">The packet to send.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> SendPacketAsync(IPacket packet, CancellationToken ct = default);

    /// <summary>
    /// Sends the given raw packet string.
    /// </summary>
    /// <param name="packetString">The packed string to send in plain text.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> SendPacketAsync(string packetString, CancellationToken ct = default);

    /// <summary>
    /// Receives the given raw packet string.
    /// </summary>
    /// <param name="packetString">The packet to receive in plain text.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> ReceivePacketAsync(string packetString, CancellationToken ct = default);

    /// <summary>
    /// Receives the given packet.
    /// </summary>
    /// <param name="packet">The packet to receive.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> ReceivePacketAsync(IPacket packet, CancellationToken ct = default);

    /// <summary>
    /// Sends the given command to the client.
    /// </summary>
    /// <remarks>
    /// Commands can be used for doing complex operations like walking that require sending multiple packets
    /// and/or calling some functions of the local client.
    /// This method will not return until the command is finished or it failed.
    /// </remarks>
    /// <param name="command">The command to send.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> SendCommandAsync(ICommand command, CancellationToken ct = default);
}