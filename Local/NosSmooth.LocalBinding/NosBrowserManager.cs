//
//  NosBrowserManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Microsoft.Extensions.Options;
using NosSmooth.LocalBinding.Errors;
using NosSmooth.LocalBinding.Options;
using NosSmooth.LocalBinding.Structs;
using Reloaded.Memory.Sigscan;
using Reloaded.Memory.Sources;
using Remora.Results;

namespace NosSmooth.LocalBinding;

/// <summary>
/// Used for browsing a nostale process data.
/// </summary>
public class NosBrowserManager
{
    /// <summary>
    /// Checks whether the given process is a NosTale client process.
    /// </summary>
    /// <remarks>
    /// This is just a guess based on presence of "NostaleData" directory.
    /// </remarks>
    /// <param name="process">The process to check.</param>
    /// <returns>Whether the process is a NosTale client.</returns>
    public static bool IsProcessNostaleProcess(Process process)
    {
        if (process.MainModule is null)
        {
            return false;
        }

        var processDirectory = Path.GetDirectoryName(process.MainModule.FileName);
        if (processDirectory is null)
        {
            return false;
        }

        return Directory.Exists(Path.Combine(processDirectory, "NostaleData"));
    }

    /// <summary>
    /// Get all running nostale processes.
    /// </summary>
    /// <returns>The nostale processes.</returns>
    public static IEnumerable<Process> GetAllNostaleProcesses()
        => Process
            .GetProcesses()
            .Where(IsProcessNostaleProcess);

    private readonly PlayerManagerOptions _playerManagerOptions;
    private readonly SceneManagerOptions _sceneManagerOptions;
    private readonly PetManagerOptions _petManagerOptions;
    private PlayerManager? _playerManager;
    private SceneManager? _sceneManager;
    private PetManagerList? _petManagerList;

    /// <summary>
    /// Initializes a new instance of the <see cref="NosBrowserManager"/> class.
    /// </summary>
    /// <param name="playerManagerOptions">The options for obtaining player manager.</param>
    /// <param name="sceneManagerOptions">The scene manager options.</param>
    /// <param name="petManagerOptions">The pet manager options.</param>
    public NosBrowserManager
    (
        IOptions<PlayerManagerOptions> playerManagerOptions,
        IOptions<SceneManagerOptions> sceneManagerOptions,
        IOptions<PetManagerOptions> petManagerOptions
    )
        : this
        (
            Process.GetCurrentProcess(),
            playerManagerOptions.Value,
            sceneManagerOptions.Value,
            petManagerOptions.Value
        )
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NosBrowserManager"/> class.
    /// </summary>
    /// <param name="process">The process to browse.</param>
    /// <param name="playerManagerOptions">The options for obtaining player manager.</param>
    /// <param name="sceneManagerOptions">The scene manager options.</param>
    /// <param name="petManagerOptions">The pet manager options.</param>
    public NosBrowserManager
    (
        Process process,
        PlayerManagerOptions playerManagerOptions,
        SceneManagerOptions sceneManagerOptions,
        PetManagerOptions petManagerOptions
    )
    {
        _playerManagerOptions = playerManagerOptions;
        _sceneManagerOptions = sceneManagerOptions;
        _petManagerOptions = petManagerOptions;
        Process = process;
        Memory = new ExternalMemory(process);
        Scanner = new Scanner(process, process.MainModule);
    }

    /// <summary>
    /// The NosTale process.
    /// </summary>
    public Process Process { get; }

    /// <summary>
    /// Gets the memory scanner.
    /// </summary>
    internal Scanner Scanner { get; }

    /// <summary>
    /// Gets the current process memory.
    /// </summary>
    internal IMemory Memory { get; }

    /// <summary>
    /// Gets whether this is a NosTale process or not.
    /// </summary>
    public bool IsNostaleProcess => NosBrowserManager.IsProcessNostaleProcess(Process);

    /// <summary>
    /// Gets whether the player is currently in game.
    /// </summary>
    /// <remarks>
    /// It may be unsafe to access some data if the player is not in game.
    /// </remarks>
    public bool IsInGame
    {
        get
        {
            var player = PlayerManager.Player;
            return player.Address != IntPtr.Zero;
        }
    }

    /// <summary>
    /// Gets the player manager.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the browser is not initialized or there was an error with initialization of player manager.</exception>
    public PlayerManager PlayerManager
    {
        get
        {
            if (_playerManager is null)
            {
                throw new InvalidOperationException
                (
                    "Could not get player manager. The browser manager is not initialized. Did you forget to call NosBrowserManager.Initialize?"
                );
            }

            return _playerManager;
        }
    }

    /// <summary>
    /// Gets the scene manager.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the browser is not initialized or there was an error with initialization of scene manager.</exception>
    public SceneManager SceneManager
    {
        get
        {
            if (_sceneManager is null)
            {
                throw new InvalidOperationException
                (
                    "Could not get scene manager. The browser manager is not initialized. Did you forget to call NosBrowserManager.Initialize?"
                );
            }

            return _sceneManager;
        }
    }

    /// <summary>
    /// Gets the pet manager list.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the browser is not initialized or there was an error with initialization of pet manager list.</exception>
    public PetManagerList PetManagerList
    {
        get
        {
            if (_petManagerList is null)
            {
                throw new InvalidOperationException
                (
                    "Could not get pet manager list. The browser manager is not initialized. Did you forget to call NosBrowserManager.Initialize?"
                );
            }

            return _petManagerList;
        }
    }

    /// <summary>
    /// Initialize the nos browser modules.
    /// </summary>
    /// <remarks>
    /// Needed to use all of the classes from NosTale.
    /// </remarks>
    /// <returns>A result that may or may not have succeeded.</returns>
    public IResult Initialize()
    {
        if (!IsNostaleProcess)
        {
            return (Result)new NotNostaleProcessError(Process);
        }

        List<IResult> errorResults = new List<IResult>();
        if (_playerManager is null)
        {
            var playerManagerResult = PlayerManager.Create(this, _playerManagerOptions);
            if (!playerManagerResult.IsSuccess)
            {
                errorResults.Add
                (
                    Result.FromError
                    (
                        new CouldNotInitializeModuleError(typeof(PlayerManager), playerManagerResult.Error),
                        playerManagerResult
                    )
                );
            }

            _playerManager = playerManagerResult.Entity;
        }

        if (_sceneManager is null)
        {
            var sceneManagerResult = SceneManager.Create(this, _sceneManagerOptions);
            if (!sceneManagerResult.IsSuccess)
            {
                errorResults.Add
                (
                    Result.FromError
                    (
                        new CouldNotInitializeModuleError(typeof(SceneManager), sceneManagerResult.Error),
                        sceneManagerResult
                    )
                );
            }

            _sceneManager = sceneManagerResult.Entity;
        }

        if (_petManagerList is null)
        {
            var petManagerResult = PetManagerList.Create(this, _petManagerOptions);
            if (!petManagerResult.IsSuccess)
            {
                errorResults.Add
                (
                    Result.FromError
                    (
                        new CouldNotInitializeModuleError(typeof(PetManagerList), petManagerResult.Error),
                        petManagerResult
                    )
                );
            }

            _petManagerList = petManagerResult.Entity;
        }

        return errorResults.Count switch
        {
            0 => Result.FromSuccess(),
            1 => errorResults[0],
            _ => (Result)new AggregateError(errorResults)
        };
    }
}