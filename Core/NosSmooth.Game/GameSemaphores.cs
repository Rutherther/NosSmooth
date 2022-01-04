//
//  GameSemaphores.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game;

/// <summary>
/// Holds information about semaphores for synchornizing the game packet data.
/// </summary>
internal class GameSemaphores
{
    private Dictionary<GameSemaphoreType, SemaphoreSlim> _semaphores;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameSemaphores"/> class.
    /// </summary>
    public GameSemaphores()
    {
        _semaphores = new Dictionary<GameSemaphoreType, SemaphoreSlim>();
        foreach (var type in Enum.GetValues(typeof(GameSemaphoreType)).Cast<GameSemaphoreType>())
        {
            _semaphores[type] = new SemaphoreSlim(1, 1);
        }
    }

    /// <summary>
    /// Acquire the given semaphore.
    /// </summary>
    /// <param name="semaphoreType">The semaphore type.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A task that may or may not have succeeded.</returns>
    public async Task AcquireSemaphore(GameSemaphoreType semaphoreType, CancellationToken ct = default)
    {
        await _semaphores[semaphoreType].WaitAsync(ct);
    }

    /// <summary>
    /// Release the acquired semaphore.
    /// </summary>
    /// <param name="semaphoreType">The semaphore type.</param>
    public void ReleaseSemaphore(GameSemaphoreType semaphoreType)
    {
        _semaphores[semaphoreType].Release();
    }
}