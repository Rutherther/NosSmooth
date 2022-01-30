//
//  Program.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Data.CLI.Commands;
using NosSmooth.Data.Database.Extensions;
using NosSmooth.Data.NOSFiles.Extensions;
using NosSmooth.Data.NOSFiles.Files;
using NosSmooth.Data.NOSFiles.Readers;
using NosSmooth.Data.NOSFiles.Readers.Types;
using Remora.Commands.Extensions;
using Remora.Commands.Services;
using Remora.Results;

namespace NosSmooth.Data.CLI;

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
        var commandService = services.GetRequiredService<CommandService>();
        var preparedCommandResult = await commandService.TryPrepareCommandAsync(string.Join(' ', arguments), services);
        if (!preparedCommandResult.IsSuccess)
        {
            Console.Error.WriteLine($"There was an error, {preparedCommandResult.Error.Message}");
            return;
        }

        if (preparedCommandResult.Entity is null)
        {
            Console.Error.WriteLine("You must enter a command such ast list or inject.");
            return;
        }

        var executionResult = await commandService.TryExecuteAsync(preparedCommandResult.Entity, services);
        if (!executionResult.Entity.IsSuccess)
        {
            switch (executionResult.Entity.Error)
            {
                case ExceptionError exc:
                    Console.Error.WriteLine($"There was an exception, {exc.Exception.Message}");
                    break;
                default:
                    Console.Error.WriteLine($"There was an error, {executionResult.Entity.Error!.Message}");
                    break;
            }
        }
    }

    private static IServiceProvider CreateServices()
    {
        var collection = new ServiceCollection();

        collection
            .AddNostaleDataParsing()
            .AddNostaleDatabaseMigrator()
            .AddCommands()
            .AddCommandTree()
            .WithCommandGroup<MigrateDatabaseCommand>()
            .WithCommandGroup<ExtractNosFileCommand>();

        return collection.BuildServiceProvider();
    }
}