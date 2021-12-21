//
//  NameChanger.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NosCore.Packets.Enumerations;
using NosCore.Packets.ServerPackets.Chats;
using NosCore.Shared.Enumerations;
using NosSmooth.Core.Client;
using NosSmooth.Core.Packets;
using NosSmooth.LocalClient;
using NosSmooth.LocalClient.Extensions;

namespace InterceptNameChanger
{
    /// <summary>
    /// Main class of name changer.
    /// </summary>
    public class NameChanger
    {
        /// <summary>
        /// Run the name changer.
        /// </summary>
        /// <returns>A task that may or may not have succeeded.</returns>
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
                .Configure<LocalClientOptions>(o => o.AllowIntercept = true)
                .AddSingleton<IPacketInterceptor, NameChangeInterceptor>()
                .BuildServiceProvider();

            var dummy1 = provider.GetRequiredService<PacketSerializerProvider>().ServerSerializer;
            var dummy2 = provider.GetRequiredService<PacketSerializerProvider>().ClientSerializer;

            var logger = provider.GetRequiredService<ILogger<NameChanger>>();
            logger.LogInformation("Hello world from NameChanger!");

            var client = provider.GetRequiredService<INostaleClient>();

            var sayResult = await client.ReceivePacketAsync(new SayPacket()
            {
                Message = "The name may be changed by typing #{NewName} into the chat.",
                VisualType = VisualType.Map,
                Type = SayColorType.Red,
                VisualId = 1,
            });

            if (!sayResult.IsSuccess)
            {
                logger.LogError("Could not send say packet");
            }

            await client.RunAsync();
        }
    }
}