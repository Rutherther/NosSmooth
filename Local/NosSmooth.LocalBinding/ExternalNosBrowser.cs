//
//  ExternalNosBrowser.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Microsoft.Extensions.Options;
using NosSmooth.LocalBinding.Options;
using NosSmooth.LocalBinding.Structs;
using Reloaded.Memory.Sigscan;
using Reloaded.Memory.Sources;
using Remora.Results;

namespace NosSmooth.LocalBinding;

/// <summary>
/// Used for browsing a nostale process data.
/// </summary>
public class ExternalNosBrowser
{
    private readonly PlayerManagerOptions _playerManagerOptions;
    private readonly SceneManagerOptions _sceneManagerOptions;
    private PlayerManager? _playerManager;
    private SceneManager? _sceneManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalNosBrowser"/> class.
    /// </summary>
    /// <param name="process">The process to browse.</param>
    /// <param name="playerManagerOptions">The options for obtaining player manager.</param>
    /// <param name="sceneManagerOptions">The scene manager options.</param>
    public ExternalNosBrowser
    (
        Process process,
        PlayerManagerOptions playerManagerOptions,
        SceneManagerOptions sceneManagerOptions
    )
    {
        _playerManagerOptions = playerManagerOptions;
        _sceneManagerOptions = sceneManagerOptions;
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
    /// Get the player manager.
    /// </summary>
    /// <returns>The player manager or an error.</returns>
    public Result<PlayerManager> GetPlayerManager()
    {
        if (_playerManager is null)
        {
            var playerManagerResult = PlayerManager.Create(this, _playerManagerOptions);
            if (!playerManagerResult.IsSuccess)
            {
                return playerManagerResult;
            }

            _playerManager = playerManagerResult.Entity;
        }

        return _playerManager;
    }

    /// <summary>
    /// Get the player manager.
    /// </summary>
    /// <returns>The player manager or an error.</returns>
    public Result<SceneManager> GetSceneManager()
    {
        if (_sceneManager is null)
        {
            var sceneManagerResult = SceneManager.Create(this, _sceneManagerOptions);
            if (!sceneManagerResult.IsSuccess)
            {
                return sceneManagerResult;
            }

            _sceneManager = sceneManagerResult.Entity;
        }

        return _sceneManager;
    }
}