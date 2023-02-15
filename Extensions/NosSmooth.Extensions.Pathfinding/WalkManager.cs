//
//  WalkManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
using NosSmooth.Core.Commands.Walking;
using NosSmooth.Core.Errors;
using NosSmooth.Extensions.Pathfinding.Errors;
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
    /// Move character to the given position.
    /// </summary>
    /// <remarks>
    /// Expect <see cref="WalkNotFinishedError"/> if the destination could not be reached.
    /// Expect <see cref="NotFoundError"/> if the path could not be found.
    /// </remarks>
    /// <param name="x">The target x coordinate.</param>
    /// <param name="y">The target y coordinate.</param>
    /// <param name="allowUserActions">Whether to allow user actions during the walk operation.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may not succeed.</returns>
    public async Task<Result> PlayerGoToAsync
    (
        short x,
        short y,
        bool allowUserActions = true,
        CancellationToken ct = default
    )
    {
        var pathResult = _pathfinder.FindPathFromCurrent(x, y);
        if (!pathResult.IsDefined(out var path))
        {
            return Result.FromError(pathResult);
        }

        return await TakePath
        (
            path,
            _state.Character,
            (x, y) => _client.SendCommandAsync
            (
                new WalkCommand
                (
                    x,
                    y,
                    2,
                    AllowUserCancel: allowUserActions
                ),
                ct
            )
        );
    }

    /// <summary>
    /// Move pet to the given position.
    /// </summary>
    /// <remarks>
    /// Expect <see cref="WalkNotFinishedError"/> if the destination could not be reached.
    /// Expect <see cref="NotFoundError"/> if the path could not be found.
    /// </remarks>
    /// <param name="mateId">The id of the mate to move.</param>
    /// <param name="x">The target x coordinate.</param>
    /// <param name="y">The target y coordinate.</param>
    /// <param name="allowUserActions">Whether to allow user actions during the walk operation.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may not succeed.</returns>
    public async Task<Result> MateWalkToAsync
    (
        long mateId,
        short x,
        short y,
        bool allowUserActions = true,
        CancellationToken ct = default
    )
    {
        if (!_state.Entities.TryGetValue(mateId, out var entityState) || entityState == _state.Character)
        {
            return new EntityStateNotFoundError(mateId);
        }

        var pathResult = _pathfinder.FindPathFromEntity(mateId, x, y);

        if (!pathResult.IsDefined(out var path))
        {
            return Result.FromError(pathResult);
        }

        return await TakePath
        (
            path,
            entityState,
            (x, y) => _client.SendCommandAsync
            (
                new MateWalkCommand
                (
                    mateId,
                    x,
                    y,
                    2,
                    AllowUserCancel: allowUserActions
                ),
                ct
            )
        );
    }

    private async Task<Result> TakePath(Path path, EntityState state, Func<short, short, Task<Result>> walkFunc)
    {
        if (path.Parts.Count == 0)
        {
            return Result.FromSuccess();
        }
        var target = path.Parts.Last();

        while (!path.ReachedEnd)
        {
            if (path.MapId != _state.MapId)
            {
                return new WalkNotFinishedError(state.X, state.Y, WalkUnfinishedReason.MapChanged);
            }

            var next = path.TakeForwardPath();
            var walkResult = await walkFunc(next.X, next.Y);
            if (!walkResult.IsSuccess)
            {
                if (path.ReachedEnd && walkResult.Error is WalkNotFinishedError walkNotFinishedError
                    && walkNotFinishedError.Reason == WalkUnfinishedReason.MapChanged)
                {
                    return Result.FromSuccess();
                }

                if (state.X == target.X && state.Y == target.Y)
                {
                    return Result.FromSuccess();
                }

                return walkResult;
            }
        }

        return Result.FromSuccess();
    }
}