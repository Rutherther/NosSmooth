//
//  WalkCommandHandler.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Options;
using NosSmooth.Core.Commands;
using NosSmooth.LocalBinding.Objects;
using NosSmooth.LocalClient.CommandHandlers.Walk.Errors;
using Remora.Results;

namespace NosSmooth.LocalClient.CommandHandlers.Walk;

/// <summary>
/// Handles <see cref="WalkCommand"/>.
/// </summary>
public class WalkCommandHandler : ICommandHandler<WalkCommand>
{
    private readonly PlayerManagerBinding _playerManagerBinding;
    private readonly WalkStatus _walkStatus;
    private readonly WalkCommandHandlerOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="WalkCommandHandler"/> class.
    /// </summary>
    /// <param name="playerManagerBinding">The character object binding.</param>
    /// <param name="walkStatus">The walk status.</param>
    /// <param name="options">The options.</param>
    public WalkCommandHandler(PlayerManagerBinding playerManagerBinding, WalkStatus walkStatus, IOptions<WalkCommandHandlerOptions> options)
    {
        _options = options.Value;
        _playerManagerBinding = playerManagerBinding;
        _walkStatus = walkStatus;
    }

    /// <inheritdoc/>
    /// 1) If client called walk, cancel.
    /// 2) If another walk command requested, cancel.
    /// 3) If at the correct spot, cancel.
    /// 4) If not walking for over x ms, cancel.
    public async Task<Result> HandleCommand(WalkCommand command, CancellationToken ct = default)
    {
        CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(ct);
        ct = linked.Token;
        await _walkStatus.SetWalking(linked, command.TargetX, command.TargetY, command.CancelOnUserMove);
        while (!ct.IsCancellationRequested)
        {
            var walkResult = _playerManagerBinding.Walk(command.TargetX, command.TargetY);
            if (!walkResult.IsSuccess)
            {
                try
                {
                    await _walkStatus.CancelWalkingAsync(ct: ct);
                }
                catch
                {
                    // ignored, just for cancellation
                }

                return Result.FromError(walkResult);
            }

            if (walkResult.Entity == false)
            {
                await _walkStatus.CancelWalkingAsync(WalkCancelReason.NosTaleReturnedFalse);
                return new WalkNotFinishedError(_walkStatus.CurrentX, _walkStatus.CurrentY, WalkCancelReason.NosTaleReturnedFalse);
            }
            try
            {
                await Task.Delay(_options.CheckDelay, ct);
            }
            catch
            {
                // ignored
            }

            if (_walkStatus.IsFinished)
            {
                return Result.FromSuccess();
            }

            if (_walkStatus.Error is not null)
            {
                return new WalkNotFinishedError(_walkStatus.CurrentX, _walkStatus.CurrentY, (WalkCancelReason)_walkStatus.Error);
            }

            if ((DateTimeOffset.Now - _walkStatus.LastWalkTime).TotalMilliseconds > _options.NotWalkingTooLongTrigger)
            {
                await _walkStatus.CancelWalkingAsync(WalkCancelReason.NotWalkingTooLong);
                return new WalkNotFinishedError(_walkStatus.CurrentX, _walkStatus.CurrentY, WalkCancelReason.NotWalkingTooLong);
            }
        }

        return new WalkNotFinishedError(_walkStatus.CurrentX, _walkStatus.CurrentY, WalkCancelReason.Unknown);
    }
}