//
//  SimpleChat.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NosCore.Packets.Enumerations;
using NosCore.Packets.ServerPackets.Chats;
using NosCore.Shared.Enumerations;
using NosSmooth.Core.Client;
using NosSmooth.Core.Packets;
using NosSmooth.LocalClient.Extensions;

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
            .AddLogging(b =>
            {
                b.ClearProviders();
                b.AddConsole();
                b.SetMinimumLevel(LogLevel.Debug);
            })
            .BuildServiceProvider();

        var dummy1 = provider.GetRequiredService<PacketSerializerProvider>().ServerSerializer;
        var dummy2 = provider.GetRequiredService<PacketSerializerProvider>().ClientSerializer;

        var logger = provider.GetRequiredService<ILogger<SimpleChat>>();
        logger.LogInformation("Hello world from SimpleChat!");

        var client = provider.GetRequiredService<INostaleClient>();

        await client.ReceivePacketAsync(new SayPacket()
        {
            Message = "Hello world from NosSmooth!",
            VisualType = VisualType.Player,
            Type = SayColorType.Red,
            VisualId = 1,
        });

        await client.RunAsync();
    }
}