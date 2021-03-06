//
//  WalkManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
using NosSmooth.Core.Commands.Walking;
using NosSmooth.Core.Errors;
using Remora.Results;

namespace NosSmooth.Extensions.Pathfinding;

/// <summary>
/// The walk manager using pathfinding to walk to given position.
/// </summary>
public class WalkManager
{
    private readonly INostaleClient _client;
    private readonly Pathfinder _pathfinder;
    private readonly PathfinderState _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="WalkManager"/> class.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="pathfinder">The pathfinder.</param>
    /// <param name="state">The state.</param>
    public WalkManager(INostaleClient client, Pathfinder pathfinder, PathfinderState state)
    {
        _client = client;
        _pathfinder = pathfinder;
        _state = state;
    }

    /// <summary>
    /// Go to the given position.
    /// </summary>
    /// <remarks>
    /// Expect <see cref="WalkNotFinishedError"/> if the destination could not be reached.
    /// Expect <see cref="NotFoundError"/> if the path could not be found.
    /// </remarks>
    /// <param name="x">The target x coordinate.</param>
    /// <param name="y">The target y coordinate.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <param name="petSelectors">The pet selectors to go with.</param>
    /// <returns>A result that may not succeed.</returns>
    public async Task<Result> GoToAsync(short x, short y, CancellationToken ct = default, params int[] petSelectors)
    {
        var pathResult = _pathfinder.FindPathFromCurrent(x, y);
        if (!pathResult.IsSuccess)
        {
            return Result.FromError(pathResult);
        }

        var path = pathResult.Entity;
        while (!path.ReachedEnd)
        {
            if (path.MapId != _state.MapId)
            {
                return new WalkNotFinishedError(_state.X, _state.Y, WalkUnfinishedReason.MapChanged);
            }

            var next = path.TakeForwardPath();
            var walkResult = await _client.SendCommandAsync(new WalkCommand(next.X, next.Y, petSelectors, 2), ct);
            if (!walkResult.IsSuccess)
            {
                if (path.ReachedEnd && walkResult.Error is WalkNotFinishedError walkNotFinishedError
                    && walkNotFinishedError.Reason == WalkUnfinishedReason.MapChanged)
                {
                    return Result.FromSuccess();
                }

                return walkResult;
            }
        }

        return Result.FromSuccess();
    }
}