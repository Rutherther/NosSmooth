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
    private readonly WinControlBindingOptions _winControlBindingOptions;
    private readonly CharacterBindingOptions _characterBindingOptions;
    private readonly NetworkBindingOptions _networkBindingOptions;
    private SceneManagerBindingOptions _sceneManagerBindingOptions;

    private NetworkBinding? _networkBinding;
    private PlayerManagerBinding? _characterBinding;
    private SceneManagerBinding? _sceneManagerBinding;
    private WinControlBinding? _windowControlBinding;

    /// <summary>
    /// Initializes a new instance of the <see cref="NosBindingManager"/> class.
    /// </summary>
    /// <param name="characterBindingOptions">The character binding options.</param>
    /// <param name="networkBindingOptions">The network binding options.</param>
    /// <param name="sceneManagerBindingOptions">The scene manager binding options.</param>
    /// <param name="winControlBindingOptions">The win control binding options.</param>
    public NosBindingManager
    (
        IOptions<CharacterBindingOptions> characterBindingOptions,
        IOptions<NetworkBindingOptions> networkBindingOptions,
        IOptions<SceneManagerBindingOptions> sceneManagerBindingOptions,
        IOptions<WinControlBindingOptions> winControlBindingOptions
    )
    {
        Hooks = new ReloadedHooks();
        Memory = new Memory();
        Scanner = new Scanner(Process.GetCurrentProcess(), Process.GetCurrentProcess().MainModule);
        _characterBindingOptions = characterBindingOptions.Value;
        _networkBindingOptions = networkBindingOptions.Value;
        _sceneManagerBindingOptions = sceneManagerBindingOptions.Value;
        _winControlBindingOptions = winControlBindingOptions.Value;
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
                    ("Could not get network. The binding manager is not initialized. Did you forget to call NosBindingManager.Initialize?");
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
                    ("Could not get character. The binding manager is not initialized. Did you forget to call NosBindingManager.Initialize?");
            }

            return _characterBinding;
        }
    }

    /// <summary>
    /// Gets the scene manager binding.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the manager is not initialized yet.</exception>
    public SceneManagerBinding SceneManager
    {
        get
        {
            if (_sceneManagerBinding is null)
            {
                throw new InvalidOperationException
                    ("Could not get scene manager. The binding manager is not initialized. Did you forget to call NosBindingManager.Initialize?");
            }

            return _sceneManagerBinding;
        }
    }

    /// <summary>
    /// Gets the window control binding.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the manager is not initialized yet.</exception>
    public WinControlBinding WinControl
    {
        get
        {
            if (_windowControlBinding is null)
            {
                throw new InvalidOperationException
                    ("Could not get window control. The binding manager is not initialized. Did you forget to call NosBindingManager.Initialize?");
            }

            return _windowControlBinding;
        }
    }

    /// <summary>
    /// Initialize the existing bindings and hook NosTale functions.
    /// </summary>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result Initialize()
    {
        List<IResult> errorResults = new List<IResult>();

        var network = NetworkBinding.Create(this, _networkBindingOptions);
        if (!network.IsSuccess)
        {
            errorResults.Add(network);
        }
        _networkBinding = network.Entity;

        var character = PlayerManagerBinding.Create(this, _characterBindingOptions);
        if (!character.IsSuccess)
        {
            errorResults.Add(character);
        }
        _characterBinding = character.Entity;

        var sceneManager = SceneManagerBinding.Create(this, _sceneManagerBindingOptions);
        if (!sceneManager.IsSuccess)
        {
            errorResults.Add(sceneManager);
        }
        _sceneManagerBinding = sceneManager.Entity;

        var winControl = WinControlBinding.Create(this, _winControlBindingOptions);
        if (!winControl.IsSuccess)
        {
            errorResults.Add(winControl);
        }
        _windowControlBinding = winControl.Entity;

        return errorResults.Count switch
        {
            0 => Result.FromSuccess(),
            _ => new AggregateError(errorResults)
        };
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