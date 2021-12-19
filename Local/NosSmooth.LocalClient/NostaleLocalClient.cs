using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Packets;
using NosSmoothCore;
using Remora.Results;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace NosSmooth.LocalClient;

public class NostaleLocalClient : BaseNostaleClient
{
    private readonly ILogger _logger;

    public NostaleLocalClient(CommandProcessor commandProcessor, IPacketSerializer packetSerializer,
        ILogger<NostaleLocalClient> logger)
        : base(commandProcessor, packetSerializer)
    {
        _logger = logger;
        NosClient = new NosClient();
    }

    public NosClient NosClient { get; }

    public override Task<Result> ReceivePacketAsync(string packetString, CancellationToken ct = default)
    {
        NosClient.GetNetwork().ReceivePacket(packetString);
        return Task.FromResult(Result.FromSuccess());
    }

    public override async Task<Result> RunAsync(CancellationToken stopRequested = default)
    {
        _logger.LogInformation("Starting local client");
        NetworkCallback receiveCallback = ReceiveCallback;
        NetworkCallback sendCallback = SendCallback;

        NosClient.GetNetwork().SetReceiveCallback(receiveCallback);
        NosClient.GetNetwork().SetSendCallback(sendCallback);
        _logger.LogInformation("Packet methods hooked successfully");

        try
        {
            await Task.Delay(-1, stopRequested);
        }
        catch
        {
        }

        NosClient.ResetHooks();

        return Result.FromSuccess();
    }

    public override Task<Result> SendPacketAsync(string packetString, CancellationToken ct = default)
    {
        NosClient.GetNetwork().SendPacket(packetString);
        return Task.FromResult(Result.FromSuccess());
    }

    private bool ReceiveCallback(string packet)
    {
        _logger.LogInformation($"Received packet {packet}");
        return true;
    }

    private bool SendCallback(string packet)
    {
        _logger.LogInformation($"Sent packet {packet}");
        return true;
    }
}