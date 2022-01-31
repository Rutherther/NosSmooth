//
//  Program.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using DataBrowser.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NosSmooth.Core.Extensions;
using NosSmooth.Data.Abstractions.Language;
using NosSmooth.Data.NOSFiles;
using NosSmooth.Data.NOSFiles.Extensions;
using NosSmooth.Data.NOSFiles.Options;
using Remora.Commands.Extensions;
using Remora.Commands.Services;

namespace DataBrowser;

/// <summary>
/// Entrypoint class.
/// </summary>
public class Program
{
    /// <summary>
    /// Entrypoint method.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task Main(string[] arguments)
    {
        var services = CreateServices();

        // Initialize the file manager that will parse the files and register the
        // ILanguageService and IInfoService into the provider.
        var fileManager = services.GetRequiredService<NostaleDataFilesManager>();
        var initializationResult = fileManager.Initialize();
        if (!initializationResult.IsSuccess)
        {
            Console.Error.WriteLine($"There was an error, {initializationResult.ToFullString()}");
            return;
        }

        // Handle the command
        var commandService = services.GetRequiredService<CommandService>();
        var preparedCommandResult = await commandService.TryPrepareCommandAsync(string.Join(' ', arguments), services);
        if (!preparedCommandResult.IsSuccess)
        {
            Console.Error.WriteLine($"There was an error, {preparedCommandResult.ToFullString()}");
            return;
        }

        if (preparedCommandResult.Entity is null)
        {
            Console.Error.WriteLine("You must enter a command such ast list or inject.");
            return;
        }

        var executionResult = await commandService.TryExecuteAsync(preparedCommandResult.Entity, services);
        if (!executionResult.IsSuccess)
        {
            Console.Error.WriteLine($"There was an error\n, {executionResult.ToFullString()}");
        }
        else if (!executionResult.Entity.IsSuccess)
        {
            Console.Error.WriteLine($"There was an error\n, {executionResult.Entity.ToFullString()}");
        }
    }

    private static IServiceProvider CreateServices()
    {
        var collection = new ServiceCollection();

        collection

            // Adds provider using .NOS files that have to be located in NostaleData folder in the starup directory.
            // Use AddNostaleDataDatabase for using the database, you have to generate it using NosSmooth.Data.CLI
            // with migrate command. It's generated from the .NOS files.
            .AddNostaleDataFiles()
            .Configure<NostaleDataOptions>
            (
                o =>
                {
                    o.SupportedLanguages = new[]
                    {
                        // These languages will be loaded into the memory from the files.
                        Language.Cz, Language.Uk
                    };

                    // The path to the data folder.
                    o.NostaleDataPath = "NostaleData";
                }
            )
            .AddCommands()
            .AddCommandTree()
            .WithCommandGroup<ItemInfoCommand>()
            .WithCommandGroup<MonsterInfoCommand>()
            .WithCommandGroup<SkillInfoCommand>();

        return collection.BuildServiceProvider();
    }
}