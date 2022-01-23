//
//  NosBindingManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Microsoft.Extensions.Options;
using NosSmooth.LocalBinding.Objects;
using NosSmooth.LocalBinding.Options;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;
using Reloaded.Memory.Sigscan;
using Reloaded.Memory.Sources;
using Remora.Results;

namespace NosSmooth.LocalBinding;

/// <summary>
/// Nostale entity binding manager.
/// </summary>
public class NosBindingManager : IDisposable
{
    private readonly CharacterBindingOptions _characterBindingOptions;
    private readonly NetworkBindingOptions _networkBindingOptions;
    private readonly ExternalNosBrowser _nosBrowser;
    private SceneManagerBindingOptions _sceneManagerBindingOptions;

    private NetworkBinding? _networkBinding;
    private PlayerManagerBinding? _characterBinding;
    private SceneManagerBinding? _sceneManagerBinding;

    /// <summary>
    /// Initializes a new instance of the <see cref="NosBindingManager"/> class.
    /// </summary>
    /// <param name="characterBindingOptions">The character binding options.</param>
    /// <param name="networkBindingOptions">The network binding options.</param>
    /// <param name="sceneManagerBindingOptions">The scene manager binding options.</param>
    /// <param name="playerManagerOptions">The player manager options.</param>
    /// <param name="sceneManagerOptions">The scene manager options.</param>
    /// <param name="petManagerOptions">The pet manager options.</param>
    public NosBindingManager
    (
        IOptions<CharacterBindingOptions> characterBindingOptions,
        IOptions<NetworkBindingOptions> networkBindingOptions,
        IOptions<SceneManagerBindingOptions> sceneManagerBindingOptions,
        IOptions<PlayerManagerOptions> playerManagerOptions,
        IOptions<SceneManagerOptions> sceneManagerOptions,
        IOptions<PetManagerOptions> petManagerOptions
    )
    {
        Hooks = new ReloadedHooks();
        Memory = new Memory();
        Scanner = new Scanner(Process.GetCurrentProcess(), Process.GetCurrentProcess().MainModule);
        _characterBindingOptions = characterBindingOptions.Value;
        _networkBindingOptions = networkBindingOptions.Value;
        _sceneManagerBindingOptions = sceneManagerBindingOptions.Value;
        _nosBrowser = new ExternalNosBrowser
        (
            Process.GetCurrentProcess(),
            playerManagerOptions.Value,
            sceneManagerOptions.Value,
            petManagerOptions.Value
        );
    }

    /// <summary>
    /// Gets the memory scanner.
    /// </summary>
    internal Scanner Scanner { get; }

    /// <summary>
    /// Gets the reloaded hooks.
    /// </summary>
    internal IReloadedHooks Hooks { get; }

    /// <summary>
    /// Gets the current process memory.
    /// </summary>
    internal IMemory Memory { get; }

    /// <summary>
    /// Gets the network binding.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the manager is not initialized yet.</exception>
    public NetworkBinding Network
    {
        get
        {
            if (_networkBinding is null)
            {
                throw new InvalidOperationException
                (
                    "Could not get network. The binding manager is not initialized. Did you forget to call NosBindingManager.Initialize?"
                );
            }

            return _networkBinding;
        }
    }

    /// <summary>
    /// Gets the character binding.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the manager is not initialized yet.</exception>
    public PlayerManagerBinding PlayerManager
    {
        get
        {
            if (_characterBinding is null)
            {
                throw new InvalidOperationException
                (
                    "Could not get character. The binding manager is not initialized. Did you forget to call NosBindingManager.Initialize?"
                );
            }

            return _characterBinding;
        }
    }

    /// <summary>
    /// Gets the character binding.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the manager is not initialized yet.</exception>
    public SceneManagerBinding SceneManager
    {
        get
        {
            if (_sceneManagerBinding is null)
            {
                throw new InvalidOperationException
                (
                    "Could not get scene manager. The binding manager is not initialized. Did you forget to call NosBindingManager.Initialize?"
                );
            }

            return _sceneManagerBinding;
        }
    }

    /// <summary>
    /// Initialize the existing bindings and hook NosTale functions.
    /// </summary>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result Initialize()
    {
        var network = NetworkBinding.Create(this, _networkBindingOptions);
        if (!network.IsSuccess)
        {
            return Result.FromError(network);
        }
        _networkBinding = network.Entity;

        var playerManager = _nosBrowser.GetPlayerManager();
        if (!playerManager.IsSuccess)
        {
            return Result.FromError(playerManager);
        }

        var sceneManager = _nosBrowser.GetSceneManager();
        if (!sceneManager.IsSuccess)
        {
            return Result.FromError(sceneManager);
        }

        var playerManagerBinding = PlayerManagerBinding.Create
        (
            this,
            playerManager.Entity,
            _characterBindingOptions
        );
        if (!playerManagerBinding.IsSuccess)
        {
            return Result.FromError(playerManagerBinding);
        }
        _characterBinding = playerManagerBinding.Entity;

        var sceneManagerBinding = SceneManagerBinding.Create
        (
            this,
            sceneManager.Entity,
            _sceneManagerBindingOptions
        );
        if (!sceneManagerBinding.IsSuccess)
        {
            return Result.FromError(sceneManagerBinding);
        }
        _sceneManagerBinding = sceneManagerBinding.Entity;

        return Result.FromSuccess();
    }

    /// <summary>
    /// Disable the currently enabled nostale hooks.
    /// </summary>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result DisableHooks()
    {
        if (_networkBinding is not null)
        {
            var result = _networkBinding.DisableHooks();
            if (!result.IsSuccess)
            {
                return result;
            }
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Scanner.Dispose();
        DisableHooks();
    }
}