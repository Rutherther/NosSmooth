//
//  Startup.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NosSmooth.Core.Client;
using NosSmooth.LocalBinding;
using NosSmooth.LocalClient;
using NosSmooth.LocalClient.Extensions;
using WalkCommands.Commands;

namespace WalkCommands;

/// <summary>
/// Startup class of WalkCommands.
/// </summary>
public class Startup
{
    private IServiceProvider BuildServices()
    {
        return new ServiceCollection()
            .AddLocalClient()
            .AddScoped<Commands.WalkCommands>()
            .AddScoped<DetachCommand>()
            .AddSingleton<CancellationTokenSource>()
            .Configure<LocalClientOptions>(o => o.AllowIntercept = true)
            .AddSingleton<IPacketInterceptor, ChatPacketInterceptor>()
            .AddLogging(b =>
            {
                b.ClearProviders();
                b.AddConsole();
                b.SetMinimumLevel(LogLevel.Debug);
            })
            .BuildServiceProvider();
    }

    /// <summary>
    /// Run the MoveToMiniland.
    /// </summary>
    /// <returns>A task that may or may not have succeeded.</returns>
    public async Task RunAsync()
    {
        var provider = BuildServices();
        var bindingManager = provider.GetRequiredService<NosBindingManager>();
        var logger = provider.GetRequiredService<ILogger<Startup>>();
        var initializeResult = bindingManager.Initialize();
        if (!initializeResult.IsSuccess)
        {
            logger.LogError($"Could not initialize {initializeResult.Error.Message}.");
        }

        var mainCancellation = provider.GetRequiredService<CancellationTokenSource>();

        var client = provider.GetRequiredService<INostaleClient>();
        await client.RunAsync(mainCancellation.Token);
    }
}