//
//  SimpleChat.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NosSmooth.Core.Client;
using NosSmooth.LocalClient.Extensions;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Chat;
using NosSmooth.Packets.Packets.Server.Chat;

namespace SimpleChat;

/// <summary>
/// The main simple chat class.
/// </summary>
public class SimpleChat
{
    /// <summary>
    /// Run the client.
    /// </summary>
    /// <returns>The task that may or may not have succeeded.</returns>
    public async Task RunAsync()
    {
        var provider = new ServiceCollection()
            .AddLocalClient()

            // .AddPacketResponder<SayResponder>()
            .AddLogging
            (
                b =>
                {
                    b.ClearProviders();
                    b.AddConsole();
                    b.SetMinimumLevel(LogLevel.Debug);
                }
            )
            .BuildServiceProvider();

        var logger = provider.GetRequiredService<ILogger<SimpleChat>>();
        logger.LogInformation("Hello world from SimpleChat!");

        var client = provider.GetRequiredService<INostaleClient>();

        await client.ReceivePacketAsync
        (
            new SayPacket(EntityType.Map, 1, SayColor.Red, "Hello world from NosSmooth!")
        );

        await client.RunAsync();
    }
}