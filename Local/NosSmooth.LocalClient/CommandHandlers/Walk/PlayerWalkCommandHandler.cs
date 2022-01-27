//
//  PlayerWalkCommandHandler.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Options;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Commands.Control;
using NosSmooth.Core.Commands.Walking;
using NosSmooth.Core.Extensions;
using NosSmooth.LocalBinding.Objects;
using NosSmooth.LocalClient.CommandHandlers.Walk.Errors;
using Remora.Results;

namespace NosSmooth.LocalClient.CommandHandlers.Walk;

/// <summary>
/// Handles <see cref="PlayerWalkCommand"/>.
/// </summary>
public class PlayerWalkCommandHandler : ICommandHandler<PlayerWalkCommand>
{
    /// <summary>
    /// Group that is used for <see cref="TakeControlCommand"/>.
    /// </summary>
    public const string PlayerWalkControlGroup = "PlayerWalk";

    private readonly PlayerManagerBinding _playerManagerBinding;
    private readonly INostaleClient _nostaleClient;
    private readonly WalkCommandHandlerOptions _options;

    private ushort _x;
    private ushort _y;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerWalkCommandHandler"/> class.
    /// </summary>
    /// <param name="playerManagerBinding">The character object binding.</param>
    /// <param name="nostaleClient">The nostale client.</param>
    /// <param name="options">The options.</param>
    public PlayerWalkCommandHandler
    (
        PlayerManagerBinding playerManagerBinding,
        INostaleClient nostaleClient,
        IOptions<WalkCommandHandlerOptions> options
    )
    {
        _options = options.Value;
        _playerManagerBinding = playerManagerBinding;
        _nostaleClient = nostaleClient;
    }

    /// <inheritdoc/>
    /// 1) If client called walk, cancel.
    /// 2) If another walk command requested, cancel.
    /// 3) If at the correct spot, cancel.
    /// 4) If not walking for over x ms, cancel.
    public async Task<Result> HandleCommand(PlayerWalkCommand command, CancellationToken ct = default)
    {
        _x = command.TargetX;
        _y = command.TargetY;

        using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(ct);
        WalkUnfinishedReason? reason = null;
        var takeControlCommand = command.CreateTakeControl
        (
            PlayerWalkControlGroup,
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

        if (reason is null && !IsAt(command.TargetX, command.TargetY))
        {
            reason = WalkUnfinishedReason.PathNotFound;
        }

        if (reason is null)
        {
            return Result.FromSuccess();
        }

        return new WalkNotFinishedError
        (
            _playerManagerBinding.PlayerManager.X,
            _playerManagerBinding.PlayerManager.Y,
            (WalkUnfinishedReason)reason
        );
    }

    private bool IsAtTarget()
    {
        return _playerManagerBinding.PlayerManager.TargetX == _playerManagerBinding.PlayerManager.Player.X
            && _playerManagerBinding.PlayerManager.TargetY == _playerManagerBinding.PlayerManager.Player.Y;
    }

    private bool IsAt(ushort x, ushort y)
    {
        return _playerManagerBinding.PlayerManager.Player.X == x && _playerManagerBinding.PlayerManager.Player.Y == y;
    }

    private async Task<Result> WalkGrantedCallback(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var result = _playerManagerBinding.Walk(_x, _y);
            if (!result.IsSuccess)
            {
                return Result.FromError(result);
            }

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