//
//  Program.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using NosSmooth.Core.Extensions;
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
        var playerManagerOptions = new PlayerManagerOptions();
        var sceneManagerOptions = new SceneManagerOptions();
        var petManagerOptions = new PetManagerOptions();

        foreach (var argument in arguments)
        {
            Process[] processes;

            if (int.TryParse(argument, out var processId))
            {
                processes = new[] { Process.GetProcessById(processId) };
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
                var externalBrowser = new NosBrowserManager
                    (process, playerManagerOptions, sceneManagerOptions, petManagerOptions);

                if (!externalBrowser.IsNostaleProcess)
                {
                    Console.Error.WriteLine($"Process {process.Id} is not a nostale process.");
                    continue;
                }

                var initializationResult = externalBrowser.Initialize();
                if (!initializationResult.IsSuccess)
                {
                    Console.Error.WriteLine(initializationResult.ToFullString());
                }

                var length = externalBrowser.PetManagerList.Length;
                Console.WriteLine(length);

                if (!externalBrowser.IsInGame)
                {
                    Console.Error.WriteLine("The player is not in game, cannot get the name of the player.");
                    continue;
                }

                Console.WriteLine($"Player in process {process.Id} is named {externalBrowser.PlayerManager.Player.Name}");
            }
        }
    }
}