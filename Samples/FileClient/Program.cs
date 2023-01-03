//
//  Program.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FileClient.Responders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Extensions;
using NosSmooth.Core.Packets;
using NosSmooth.Data.Abstractions.Language;
using NosSmooth.Data.NOSFiles.Extensions;
using NosSmooth.Data.NOSFiles.Options;
using NosSmooth.Game.Extensions;
using NosSmooth.Packets;
using NosSmooth.PacketSerializer;

namespace FileClient;

/// <summary>
/// An entrypoint class.
/// </summary>
public static class Program
{
    // TODO: create console hosting.

    /// <summary>
    /// An entrypoint method.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task Main(string[] args)
    {
        await using FileStream stream = File.OpenRead(string.Join(' ', args));
        await CreateHost(stream).StartAsync();
    }

    private static IHost CreateHost(Stream fileStream)
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices(coll =>
            {
                coll.AddHostedService<App>();

                coll.AddNostaleCore()
                    .AddNostaleGame()
                    .AddNostaleDataFiles()
                    .AddPacketResponder<PacketNotFoundResponder>()
                    .Configure<LanguageServiceOptions>(o => o.Language = Language.Cz)
                    .Configure<NostaleDataOptions>(o => o.SupportedLanguages = new[]
                    {
                        Language.Cz
                    });
                coll.AddSingleton<INostaleClient>(p => new Client(
                    fileStream,
                    p.GetRequiredService<PacketHandler>(),
                    p.GetRequiredService<CommandProcessor>(),
                    p.GetRequiredService<IPacketSerializer>(),
                    p.GetRequiredService<ILogger<Client>>()
                ));
            })
            .UseConsoleLifetime()
            .Build();
    }
}