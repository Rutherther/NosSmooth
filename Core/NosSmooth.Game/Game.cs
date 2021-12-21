//
//  Game.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NosSmooth.Game.Data.Chat;
using NosSmooth.Game.Data.Inventory;
using NosSmooth.Game.Data.Raids;
using NosSmooth.Game.Entities;
using NosSmooth.Game.Maps;

namespace NosSmooth.Game;

/// <summary>
/// Represents base nostale game class with the character, current map, friends and current raid.
/// </summary>
public class Game
{
    private Map? _currentMap;
    private readonly GameOptions _options;

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
    /// Gets the friends list of the current client.
    /// </summary>
    public IReadOnlyList<Friend>? Friends { get; internal set; }

    /// <summary>
    /// Gets the active raid the client is currently on.
    /// </summary>
    /// <remarks>
    /// May be null if there is no raid in progress.
    /// </remarks>
    public Raid? CurrentRaid { get; internal set; }

    /// <summary>
    /// Gets the inventory of the client character.
    /// </summary>
    public Inventory Inventory { get; internal set; }

    /// <summary>
    /// Cancellation token for changing the map to use in memory cache.
    /// </summary>
    internal CancellationTokenSource? MapChanged { get; private set; }
}