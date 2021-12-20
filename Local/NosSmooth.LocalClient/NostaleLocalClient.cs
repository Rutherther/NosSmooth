//
//  NostaleLocalClient.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Packets;
using NosSmoothCore;
using Remora.Results;

namespace NosSmooth.LocalClient;

/// <summary>
/// The local nostale client.
/// </summary>
/// <remarks>
/// Client used for living in the same process as NostaleClientX.exe.
/// It hooks the send and receive packet methods.
/// </remarks>
public class NostaleLocalClient : BaseNostaleClient
{
    private readonly IPacketSerializer _packetSerializer;
    private readonly IPacketHandler _packetHandler;
    private readonly ILogger _logger;
    private readonly NosClient _client;
    private readonly LocalClientOptions _options;
    private readonly IPacketInterceptor? _interceptor;

    /// <summary>
    /// Initializes a new instance of the <see cref="NostaleLocalClient"/> class.
    /// </summary>
    /// <param name="commandProcessor">The command processor.</param>
    /// <param name="packetSerializer">The packet serializer.</param>
    /// <param name="packetHandler">The packet handler.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The options for the client.</param>
    /// <param name="provider">The dependency injection provider.</param>
    /// <param name="client">The nostale managed client.</param>
    public NostaleLocalClient
    (
        CommandProcessor commandProcessor,
        IPacketSerializer packetSerializer,
        IPacketHandler packetHandler,
        ILogger<NostaleLocalClient> logger,
        IOptions<LocalClientOptions> options,
        IServiceProvider provider,
        NosClient client
    )
        : base(commandProcessor, packetSerializer)
    {
        _options = options.Value;
        _packetSerializer = packetSerializer;
        _packetHandler = packetHandler;
        _logger = logger;
        _client = client;

        if (_options.AllowIntercept)
        {
            _interceptor = provider.GetRequiredService<IPacketInterceptor>();
        }
    }

    /// <inheritdoc />
    public override async Task<Result> RunAsync(CancellationToken stopRequested = default)
    {
        _logger.LogInformation("Starting local client");
        NetworkCallback receiveCallback = ReceiveCallback;
        NetworkCallback sendCallback = SendCallback;

        _client.GetNetwork().SetReceiveCallback(receiveCallback);
        _client.GetNetwork().SetSendCallback(sendCallback);
        _logger.LogInformation("Packet methods hooked successfully");

        try
        {
            await Task.Delay(-1, stopRequested);
        }
        catch
        {
        }

        _client.ResetHooks();

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public override Task<Result> ReceivePacketAsync(string packetString, CancellationToken ct = default)
    {
        ReceivePacket(packetString);
        return Task.FromResult(Result.FromSuccess());
    }

    /// <inheritdoc />
    public override Task<Result> SendPacketAsync(string packetString, CancellationToken ct = default)
    {
        SendPacket(packetString);
        return Task.FromResult(Result.FromSuccess());
    }

    private bool ReceiveCallback(string packet)
    {
        if (_options.AllowIntercept)
        {
            if (_interceptor is null)
            {
                throw new InvalidOperationException("The interceptor cannot be null if interception is allowed.");
            }

            return _interceptor.InterceptReceive(ref packet);
        }

        // TODO: handlers

        _logger.LogInformation($"Received packet {packet}");
        return true;
    }

    private bool SendCallback(string packet)
    {
        if (_options.AllowIntercept)
        {
            if (_interceptor is null)
            {
                throw new InvalidOperationException("The interceptor cannot be null if interception is allowed.");
            }

            return _interceptor.InterceptSend(ref packet);
        }

        // TODO: handlers

        _logger.LogInformation($"Sent packet {packet}");
        return true;
    }

    private void SendPacket(string packetString)
    {
        _client.GetNetwork().SendPacket(packetString);
    }

    private void ReceivePacket(string packetString)
    {
        _client.GetNetwork().ReceivePacket(packetString);
    }
}