//
//  Program.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using NosSmooth.LocalBinding;
using NosSmooth.LocalBinding.Options;

namespace ExternalBrowser;

/// <summary>
/// The entrypoint class for ExternalBrowser.
/// </summary>
public class Program
{
    /// <summary>
    /// The entrypoint method for ExternalBrowser.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    public static void Main(string[] arguments)
    {
        var playerManagerOptions = new CharacterBindingOptions();
        var sceneManagerOptions = new SceneManagerBindingOptions();

        foreach (var argument in arguments)
        {
            Process[] processes;

            if (int.TryParse(argument, out var processId))
            {
                processes = new Process[] { Process.GetProcessById(processId) };
            }
            else
            {
                processes = Process
                    .GetProcesses()
                    .Where(x => x.ProcessName.Contains(argument))
                    .ToArray();
            }

            foreach (var process in processes)
            {
                var externalBrowser = new ExternalNosBrowser(process, playerManagerOptions, sceneManagerOptions);
                var playerManager = externalBrowser.GetPlayerManager();
                if (!playerManager.IsSuccess)
                {
                    Console.Error.WriteLine($"Could not get the player manager: {playerManager.Error.Message}");
                    continue;
                }

                Console.WriteLine($"Player in process {process.Id} is named {playerManager.Entity.Player.Name}");
            }
        }
    }
}