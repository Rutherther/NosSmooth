//
//  NameChanger.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NosSmooth.Core.Client;
using NosSmooth.LocalClient;
using NosSmooth.LocalClient.Extensions;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Chat;
using NosSmooth.Packets.Server.Chat;

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
                .AddLogging
                (
                    b =>
                    {
                        b.ClearProviders();
                        b.AddConsole();
                        b.SetMinimumLevel(LogLevel.Debug);
                    }
                )
                .Configure<LocalClientOptions>(o => o.AllowIntercept = true)
                .AddSingleton<IPacketInterceptor, NameChangeInterceptor>()
                .BuildServiceProvider();

            var logger = provider.GetRequiredService<ILogger<NameChanger>>();
            logger.LogInformation("Hello world from NameChanger!");

            var client = provider.GetRequiredService<INostaleClient>();

            var sayResult = await client.ReceivePacketAsync
            (
                new SayPacket
                    (EntityType.Map, 1, SayColor.Red, "The name may be changed by typing #{NewName} into the chat.")
            );

            if (!sayResult.IsSuccess)
            {
                logger.LogError("Could not send say packet");
            }

            await client.RunAsync();
        }
    }
}