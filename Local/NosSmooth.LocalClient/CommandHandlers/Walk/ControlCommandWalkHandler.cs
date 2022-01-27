//
//  ControlCommandWalkHandler.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
using NosSmooth.Core.Commands.Control;
using NosSmooth.Core.Commands.Walking;
using NosSmooth.Core.Extensions;
using NosSmooth.LocalBinding.Structs;
using NosSmooth.LocalClient.CommandHandlers.Walk.Errors;
using Remora.Results;

namespace NosSmooth.LocalClient.CommandHandlers.Walk;

/// <summary>
/// Handler for control manager walk command.
/// </summary>
internal class ControlCommandWalkHandler
{
    private readonly INostaleClient _nostaleClient;
    private readonly Func<ushort, ushort, Result<bool>> _walkFunction;
    private readonly ControlManager _controlManager;
    private readonly WalkCommandHandlerOptions _options;

    private ushort _x;
    private ushort _y;

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlCommandWalkHandler"/> class.
    /// </summary>
    /// <param name="nostaleClient">The nostale client.</param>
    /// <param name="walkFunction">The walk function.</param>
    /// <param name="controlManager">The control manager.</param>
    /// <param name="options">The options.</param>
    public ControlCommandWalkHandler
    (
        INostaleClient nostaleClient,
        Func<ushort, ushort, Result<bool>> walkFunction,
        ControlManager controlManager,
        WalkCommandHandlerOptions options
    )
    {
        _nostaleClient = nostaleClient;
        _walkFunction = walkFunction;
        _controlManager = controlManager;
        _options = options;
    }

    /// <summary>
    /// Handle walk take control command.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="command">The take control command.</param>
    /// <param name="groupName">The name of the take control group.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<Result> HandleCommand(ushort x, ushort y, ITakeControlCommand command, string groupName, CancellationToken ct = default)
    {
        _x = x;
        _y = y;

        using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(ct);
        WalkUnfinishedReason? reason = null;
        var takeControlCommand = command.CreateTakeControl
        (
            groupName,
            WalkGrantedCallback,
            (r) =>
            {
                reason = r switch
                {
                    ControlCancelReason.AnotherTask => WalkUnfinishedReason.AnotherTask,
                    ControlCancelReason.UserAction => WalkUnfinishedReason.UserAction,
                    _ => WalkUnfinishedReason.Unknown
                };
                return Task.FromResult(Result.FromSuccess());
            }
        );

        var commandResult = await _nostaleClient.SendCommandAsync(takeControlCommand, ct);
        if (!commandResult.IsSuccess)
        {
            return commandResult;
        }

        if (reason is null && !IsAt(x, y))
        {
            reason = WalkUnfinishedReason.PathNotFound;
        }

        if (reason is null)
        {
            return Result.FromSuccess();
        }

        return new WalkNotFinishedError
        (
            _controlManager.X,
            _controlManager.Y,
            (WalkUnfinishedReason)reason
        );
    }

    private bool IsAtTarget()
    {
        return _controlManager.TargetX == _controlManager.Entity.X
            && _controlManager.TargetY == _controlManager.Entity.Y;
    }

    private bool IsAt(ushort x, ushort y)
    {
        return _controlManager.Entity.X == x && _controlManager.Entity.Y == y;
    }

    private async Task<Result> WalkGrantedCallback(CancellationToken ct)
    {
        var result = _walkFunction(_x, _y);
        if (!result.IsSuccess)
        {
            return Result.FromError(result);
        }

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_options.CheckDelay, ct);
            }
            catch
            {
                // ignored
            }

            if (IsAtTarget() || IsAt(_x, _y))
            {
                return Result.FromSuccess();
            }
        }

        return Result.FromSuccess(); // cancellation is handled in cancellation callback.
    }
}