//
//  ListProcessesCommand.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Results;

namespace NosSmooth.Injector.CLI.Commands
{
    /// <summary>
    /// Command for listing processes to find id of NosTale process.
    /// </summary>
    internal class ListProcessesCommand : CommandGroup
    {
        /// <summary>
        /// Lists processes by the given criteria.
        /// </summary>
        /// <param name="nameContains">What should the name of the process contain.</param>
        /// <returns>A result that may or may not have succeeded.</returns>
        [Command("list")]
        public Task<Result> List(string nameContains = "Nostale")
        {
            var processes = Process.GetProcesses();
            foreach (var process in processes.Where(x => x.ProcessName.Contains(nameContains, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine(ProcessToString(process));
            }

            return Task.FromResult(Result.FromSuccess());
        }

        private string ProcessToString(Process process)
        {
            return $"{process.ProcessName} - {process.Id}";
        }
    }
}
