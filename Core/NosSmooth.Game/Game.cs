//
//  Game.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Options;
using NosSmooth.Core.Client;
using NosSmooth.Core.Stateful;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Maps;
using NosSmooth.Game.Data.Raids;

namespace NosSmooth.Game;

/// <summary>
/// Represents base nostale game class with the character, current map, friends and current raid.
/// </summary>
public class Game : IStatefulEntity
{
    private readonly GameOptions _options;
    private Map? _currentMap;

    /// <summary>
    /// Initializes a new instance of the <see cref="Game"/> class.
    /// </summary>
    /// <param name="options">The options for the game.</param>
    public Game(IOptions<GameOptions> options)
    {
        Semaphores = new GameSemaphores();
        _options = options.Value;
    }

    /// <summary>
    /// Gets the game semaphores.
    /// </summary>
    internal GameSemaphores Semaphores { get; }

    /// <summary>
    /// Gets the playing character of the client.
    /// </summary>
    public Character? Character { get; internal set; }

    /// <summary>
    /// Gets the current map of the client.
    /// </summary>
    /// <remarks>
    /// Will be null until current map packet is received.
    /// </remarks>
    public Map? CurrentMap
    {
        get => _currentMap;
        internal set
        {
            _currentMap = value;
        }
    }

    /// <summary>
    /// Gets the active raid the client is currently on.
    /// </summary>
    /// <remarks>
    /// May be null if there is no raid in progress.
    /// </remarks>
    public Raid? CurrentRaid { get; internal set; }

    /// <summary>
    /// Creates the character if it is null, or updates the current character.
    /// </summary>
    /// <param name="create">The function for creating the character.</param>
    /// <param name="update">The function for updating the character.</param>
    /// <param name="releaseSemaphore">Whether to release the semaphore used for changing the character.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>The updated character.</returns>
    internal async Task<Character> CreateOrUpdateCharacterAsync
    (
        Func<Character> create,
        Func<Character, Character> update,
        bool releaseSemaphore = true,
        CancellationToken ct = default
    )
    {
        return await CreateOrUpdateAsync
        (
            GameSemaphoreType.Character,
            () => Character,
            c => Character = c,
            create,
            update,
            releaseSemaphore,
            ct
        );
    }

    /// <summary>
    /// Creates the map if it is null, or updates the current map.
    /// </summary>
    /// <param name="create">The function for creating the map.</param>
    /// <param name="releaseSemaphore">Whether to release the semaphore used for changing the map.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>The updated character.</returns>
    internal async Task<Map?> CreateMapAsync
    (
        Func<Map?> create,
        bool releaseSemaphore = true,
        CancellationToken ct = default
    )
    {
        return await CreateAsync
        (
            GameSemaphoreType.Map,
            m => CurrentMap = m,
            create,
            releaseSemaphore,
            ct
        );
    }

    /// <summary>
    /// Creates the map if it is null, or updates the current map.
    /// </summary>
    /// <param name="create">The function for creating the map.</param>
    /// <param name="update">The function for updating the map.</param>
    /// <param name="releaseSemaphore">Whether to release the semaphore used for changing the map.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>The updated character.</returns>
    internal async Task<Map?> CreateOrUpdateMapAsync
    (
        Func<Map?> create,
        Func<Map?, Map?> update,
        bool releaseSemaphore = true,
        CancellationToken ct = default
    )
    {
        return await CreateOrUpdateAsync<Map?>
        (
            GameSemaphoreType.Map,
            () => CurrentMap,
            m => CurrentMap = m,
            create,
            update,
            releaseSemaphore,
            ct
        );
    }

    private async Task<T> CreateAsync<T>
    (
        GameSemaphoreType type,
        Action<T> set,
        Func<T> create,
        bool releaseSemaphore = true,
        CancellationToken ct = default
    )
    {
        await Semaphores.AcquireSemaphore(type, ct);

        try
        {
            var current = create();
            set(current);
            return current;
        }
        finally
        {
            if (releaseSemaphore)
            {
                Semaphores.ReleaseSemaphore(type);
            }
        }
    }

    private async Task<T> CreateOrUpdateAsync<T>
    (
        GameSemaphoreType type,
        Func<T?> get,
        Action<T> set,
        Func<T> create,
        Func<T, T> update,
        bool releaseSemaphore = true,
        CancellationToken ct = default
    )
    {
        await Semaphores.AcquireSemaphore(type, ct);

        try
        {
            var current = get();
            if (current is null)
            {
                current = create();
            }
            else
            {
                current = update(current);
            }

            set(current);
            return current;
        }
        finally
        {
            if (releaseSemaphore)
            {
                Semaphores.ReleaseSemaphore(type);
            }
        }
    }
}