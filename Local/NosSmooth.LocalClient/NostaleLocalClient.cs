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
using NosSmooth.Core.Extensions;
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
    private readonly PacketSerializerProvider _packetSerializerProvider;
    private readonly IPacketHandler _packetHandler;
    private readonly ILogger _logger;
    private readonly IServiceProvider _provider;
    private readonly NosClient _client;
    private readonly LocalClientOptions _options;
    private IPacketInterceptor? _interceptor;

    /// <summary>
    /// Initializes a new instance of the <see cref="NostaleLocalClient"/> class.
    /// </summary>
    /// <param name="commandProcessor">The command processor.</param>
    /// <param name="packetSerializer">The packet serializer.</param>
    /// <param name="packetSerializerProvider">The packet serializer provider.</param>
    /// <param name="packetHandler">The packet handler.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The options for the client.</param>
    /// <param name="provider">The dependency injection provider.</param>
    /// <param name="client">The nostale managed client.</param>
    public NostaleLocalClient
    (
        CommandProcessor commandProcessor,
        IPacketSerializer packetSerializer,
        PacketSerializerProvider packetSerializerProvider,
        IPacketHandler packetHandler,
        ILogger<NostaleLocalClient> logger,
        IOptions<LocalClientOptions> options,
        IServiceProvider provider,
        NosClient client
    )
        : base(commandProcessor, packetSerializer)
    {
        _options = options.Value;
        _packetSerializerProvider = packetSerializerProvider;
        _packetHandler = packetHandler;
        _logger = logger;
        _provider = provider;
        _client = client;
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
                _interceptor = _provider.GetRequiredService<IPacketInterceptor>();
            }

            return _interceptor.InterceptReceive(ref packet);
        }

        Task.Run(async () => await ProcessPacketAsync(PacketType.Received, packet));

        return true;
    }

    private bool SendCallback(string packet)
    {
        if (_options.AllowIntercept)
        {
            if (_interceptor is null)
            {
                _interceptor = _provider.GetRequiredService<IPacketInterceptor>();
            }

            return _interceptor.InterceptSend(ref packet);
        }

        Task.Run(async () => await ProcessPacketAsync(PacketType.Sent, packet));

        return true;
    }

    private void SendPacket(string packetString)
    {
        _client.GetNetwork().SendPacket(packetString);
        _logger.LogDebug($"Sending client packet {packetString}");
    }

    private void ReceivePacket(string packetString)
    {
        _client.GetNetwork().ReceivePacket(packetString);
        _logger.LogDebug($"Receiving client packet {packetString}");
    }

    private async Task ProcessPacketAsync(PacketType type, string packetString)
    {
        IPacketSerializer serializer;
        if (type == PacketType.Received)
        {
            serializer = _packetSerializerProvider.ServerSerializer;
        }
        else
        {
            serializer = _packetSerializerProvider.ClientSerializer;
        }

        var packet = serializer.Deserialize(packetString);
        if (!packet.IsSuccess)
        {
            _logger.LogWarning($"Could not parse {packetString}. Reason: {packet.Error.Message}");
            return;
        }

        Result result;
        if (type == PacketType.Received)
        {
            result = await _packetHandler.HandleReceivedPacketAsync(packet.Entity, packetString);
        }
        else
        {
            result = await _packetHandler.HandleSentPacketAsync(packet.Entity, packetString);
        }

        if (!result.IsSuccess)
        {
            _logger.LogError("There was an error whilst handling packet");
            _logger.LogResultError(result);
        }
    }
}