//
//  ChatInterceptor.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosCore.Packets.Enumerations;
using NosSmooth.Game.Apis;
using NosSmooth.LocalClient;

namespace SimplePiiBot;

/// <summary>
/// The chat interceptor for handling commands.
/// </summary>
public class ChatInterceptor : IPacketInterceptor
{
    private readonly PiiBot _bot;
    private readonly NostaleChatPacketApi _chatApi;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatInterceptor"/> class.
    /// </summary>
    /// <param name="bot">The pii bot.</param>
    /// <param name="chatApi">The chat packet api.</param>
    public ChatInterceptor(PiiBot bot, NostaleChatPacketApi chatApi)
    {
        _bot = bot;
        _chatApi = chatApi;
    }

    /// <inheritdoc />
    public bool InterceptSend(ref string packet)
    {
        if (packet.StartsWith("say #"))
        {
            var command = packet.Substring(5);
            if (command == "enable")
            {
                if (_bot.EnableBot())
                {
                    _chatApi.ReceiveSystemMessageAsync("The bot was enabled!")
                        .GetAwaiter().GetResult();
                }
                else
                {
                    _chatApi.ReceiveSystemMessageAsync("Could not enable the bot!", SayColorType.Red)
                        .GetAwaiter().GetResult();
                }
            }
            else if (command == "disable")
            {
                if (_bot.DisableBot())
                {
                    _chatApi.ReceiveSystemMessageAsync("The bot was disabled!")
                        .GetAwaiter().GetResult();
                }
                else
                {
                    _chatApi.ReceiveSystemMessageAsync("Could not enable the bot!", SayColorType.Red)
                        .GetAwaiter().GetResult();
                }
            }
            else
            {
                _chatApi.ReceiveSystemMessageAsync("Unknown command.", SayColorType.Red)
                    .GetAwaiter().GetResult();
            }

            return false;
        }

        return true;
    }

    /// <inheritdoc />
    public bool InterceptReceive(ref string packet)
    {
        return true;
    }
}