//
//  ChatPacketInterceptor.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NosCore.Packets.Enumerations;
using NosCore.Packets.ServerPackets.Chats;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Extensions;
using NosSmooth.LocalClient;
using Remora.Results;
using WalkCommands.Commands;

namespace WalkCommands;

/// <summary>
/// Interceptor of chat commands.
/// </summary>
public class ChatPacketInterceptor : IPacketInterceptor
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<ChatPacketInterceptor> _logger;
    private readonly INostaleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatPacketInterceptor"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="client">The nostale client.</param>
    public ChatPacketInterceptor(IServiceProvider provider, ILogger<ChatPacketInterceptor> logger, INostaleClient client)
    {
        _provider = provider;
        _logger = logger;
        _client = client;
    }

    /// <inheritdoc />
    public bool InterceptSend(ref string packet)
    {
        if (packet.StartsWith($"say #"))
        {
            var packetString = packet;
            Task.Run(async () =>
            {
                try
                {
                    await ExecuteCommand(packetString.Substring(5));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Could not execute command.");
                }
            });
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    public bool InterceptReceive(ref string packet)
    {
        return true;
    }

    private async Task ExecuteCommand(string command)
    {
        await _client.ReceivePacketAsync(new SayPacket
        {
            Type = SayColorType.Green, Message = $"Handling a command {command}."
        });

        var splitted = command.Split(new[] { ' ' });
        using var scope = _provider.CreateScope();
        Result result;
        switch (splitted[0])
        {
            case "walk":
                var walkCommand = scope.ServiceProvider.GetRequiredService<Commands.WalkCommands>();
                result = await walkCommand.HandleWalkToAsync(int.Parse(splitted[1]), int.Parse(splitted[2]), splitted.Length > 3 ? bool.Parse(splitted[3]) : true);
                break;
            case "detach":
                var detachCommand = scope.ServiceProvider.GetRequiredService<DetachCommand>();
                result = await detachCommand.HandleDetach();
                break;
            default:
                await _client.ReceivePacketAsync(new SayPacket
                {
                    Type = SayColorType.Red, Message = $"The command {splitted[0]} was not found."
                });
                return;
        }

        if (!result.IsSuccess)
        {
            _logger.LogError("Could not execute a command");
            _logger.LogResultError(result);
        }
    }
}