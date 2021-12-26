//
//  PiiBot.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NosSmooth.Core.Client;
using NosSmooth.Core.Packets;
using NosSmooth.Game.Events.Characters;
using NosSmooth.Game.Extensions;
using NosSmooth.LocalClient;
using NosSmooth.LocalClient.Extensions;
using SimplePiiBot.Responders;

namespace SimplePiiBot;

/// <summary>
/// The pii bot logic.
/// </summary>
public class PiiBot
{
    private IServiceProvider SetupServices()
    {
        return new ServiceCollection()
            .AddNostaleGame()
            .AddLocalClient()
            .AddGameResponder<SkillsReceivedResponder>()
            .AddSingleton(this)
            .AddSingleton<IPacketInterceptor, ChatInterceptor>()
            .AddLogging(b =>
            {
                b.ClearProviders();
                b.AddConsole();
                b.SetMinimumLevel(LogLevel.Debug);
            })
            .BuildServiceProvider();
    }

    /// <summary>
    /// Run the bot.
    /// </summary>
    /// <returns>A task that may or may not have succeeded.</returns>
    public Task RunAsync()
    {
        var provider = SetupServices();

        return provider.GetRequiredService<INostaleClient>().RunAsync();
    }

    /// <summary>
    /// Enable the bot function.
    /// </summary>
    /// <returns>Whether the bot was enabled.</returns>
    public bool EnableBot()
    {
        return true;
    }

    /// <summary>
    /// Disable the bot function.
    /// </summary>
    /// <returns>Whether the bot was disabled.</returns>
    public bool DisableBot()
    {
        return true;
    }
}