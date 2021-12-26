//
//  Game.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Options;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Maps;
using NosSmooth.Game.Data.Raids;

namespace NosSmooth.Game;

/// <summary>
/// Represents base nostale game class with the character, current map, friends and current raid.
/// </summary>
public class Game
{
    private readonly GameOptions _options;
    private Map? _currentMap;

    /// <summary>
    /// Initializes a new instance of the <see cref="Game"/> class.
    /// </summary>
    /// <param name="options">The options for the game.</param>
    public Game(IOptions<GameOptions> options)
    {
        _options = options.Value;
    }

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
            MapChanged?.CancelAfter(TimeSpan.FromSeconds(_options.EntityCacheDuration));
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
    /// Cancellation token for changing the map to use in memory cache.
    /// </summary>
    internal CancellationTokenSource? MapChanged { get; private set; }

    /// <summary>
    /// Gets the set semaphore used for changing internal fields.
    /// </summary>
    internal SemaphoreSlim SetSemaphore { get; } = new SemaphoreSlim(1, 1);

    /// <summary>
    /// Ensures that Character is not null.
    /// </summary>
    /// <param name="releaseSemaphore">Whether to release the semaphore.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A Task that may or may not have succeeded.</returns>
    internal async Task<Character> EnsureCharacterCreatedAsync(bool releaseSemaphore, CancellationToken ct = default)
    {
        await SetSemaphore.WaitAsync(ct);
        Character? character = Character;
        if (Character is null)
        {
            Character = character = new Character();
        }

        if (releaseSemaphore)
        {
            SetSemaphore.Release();
        }

        return character!;
    }
}