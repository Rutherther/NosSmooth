//
//  Program.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Injector.CLI.Commands;
using Remora.Commands.Extensions;
using Remora.Commands.Services;
using Remora.Results;

namespace NosSmooth.Injector.CLI
{
    /// <summary>
    /// The entrypoint class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entrypoint method.
        /// </summary>
        /// <param name="argv">The command line arguments.</param>
        /// <returns>A task that may or may not have succeeded.</returns>
        public static async Task Main(string[] argv)
        {
            var services = CreateServices();
            var commandService = services.GetRequiredService<CommandService>();
            var preparedCommandResult = await commandService.TryPrepareCommandAsync(string.Join(' ', argv), services);
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

                return;
            }
        }

        private static IServiceProvider CreateServices()
        {
            var collection = new ServiceCollection();
            collection
                .AddSingleton<NosInjector>()
                .AddOptions<NosInjectorOptions>();

            collection
                .AddCommands()
                .AddCommandTree()
                    .WithCommandGroup<InjectCommand>()
                    .WithCommandGroup<ListProcessesCommand>();

            return collection.BuildServiceProvider();
        }
    }
}
