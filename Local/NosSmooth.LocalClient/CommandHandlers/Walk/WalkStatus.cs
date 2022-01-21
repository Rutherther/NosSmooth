//
//  WalkStatus.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.LocalBinding.Objects;

namespace NosSmooth.LocalClient.CommandHandlers.Walk;

/// <summary>
/// The status for <see cref="WalkCommandHandler"/>.
/// </summary>
public class WalkStatus
{
    private readonly PlayerManagerBinding _playerManagerBinding;
    private readonly SemaphoreSlim _semaphore;
    private CancellationTokenSource? _walkingCancellation;
    private bool _userCanCancel;
    private bool _walkHooked;

    /// <summary>
    /// Initializes a new instance of the <see cref="WalkStatus"/> class.
    /// </summary>
    /// <param name="playerManagerBinding">The character binding.</param>
    public WalkStatus(PlayerManagerBinding playerManagerBinding)
    {
        _playerManagerBinding = playerManagerBinding;
        _semaphore = new SemaphoreSlim(1, 1);
    }

    /// <summary>
    /// Gets if the walk command is in progress.
    /// </summary>
    public bool IsWalking => _walkingCancellation is not null;

    /// <summary>
    /// Gets if the current walk command has been finished.
    /// </summary>
    public bool IsFinished { get; private set; }

    /// <summary>
    /// Gets the last time of walk.
    /// </summary>
    public DateTimeOffset LastWalkTime { get; private set; }

    /// <summary>
    /// Gets the walk target x coordinate.
    /// </summary>
    public int TargetX { get; private set; }

    /// <summary>
    /// Gets the walk target y coordinate.
    /// </summary>
    public int TargetY { get; private set; }

    /// <summary>
    /// Gets the characters current x coordinate.
    /// </summary>
    public int? CurrentX { get; private set; }

    /// <summary>
    /// Gets the characters current y coordinate.
    /// </summary>
    public int? CurrentY { get; private set; }

    /// <summary>
    /// Gets the error cause of cancellation.
    /// </summary>
    public WalkCancelReason? Error { get; private set; }

    /// <summary>
    /// Update the time of last walk, called on WalkPacket.
    /// </summary>
    /// <param name="currentX">The current characters x coordinate.</param>
    /// <param name="currentY">The current characters y coordinate.</param>
    internal void UpdateWalkTime(int currentX, int currentY)
    {
        CurrentX = currentX;
        CurrentY = currentY;
        LastWalkTime = DateTimeOffset.Now;
    }

    /// <summary>
    /// Sets that the walk command handler is handling walk command.
    /// </summary>
    /// <param name="cancellationTokenSource">The cancellation token source for cancelling the operation.</param>
    /// <param name="targetX">The walk target x coordinate.</param>
    /// <param name="targetY">The walk target y coordinate.</param>
    /// <param name="userCanCancel">Whether the user can cancel the operation by moving elsewhere.</param>
    /// <returns>A task that may or may not have succeeded.</returns>
    internal async Task SetWalking(CancellationTokenSource cancellationTokenSource, int targetX, int targetY, bool userCanCancel)
    {
        await _semaphore.WaitAsync(cancellationTokenSource.Token);
        if (IsWalking)
        {
            // Cannot call CancelWalkingAsync as that would result in a deadlock
            _walkingCancellation?.Cancel();
            _walkingCancellation = null;
        }

        IsFinished = false;
        Error = null;
        TargetX = targetX;
        TargetY = targetY;
        CurrentX = CurrentY = null;
        _walkingCancellation = cancellationTokenSource;
        LastWalkTime = DateTime.Now;
        _userCanCancel = userCanCancel;

        if (!_walkHooked)
        {
            _playerManagerBinding.WalkCall += OnCharacterWalked;
            _walkHooked = true;
        }

        _semaphore.Release();
    }

    /// <summary>
    /// Cancel the walking token.
    /// </summary>
    /// <param name="error">The cause.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A task that may or may not have succeeded.</returns>
    internal async Task CancelWalkingAsync(WalkCancelReason? error = null, CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        if (!IsWalking)
        {
            _semaphore.Release();
            return;
        }

        Error = error;
        try
        {
            _walkingCancellation?.Cancel();
        }
        catch
        {
            // ignored
        }

        _walkingCancellation = null;
        _semaphore.Release();
    }

    /// <summary>
    /// Finish the walk successfully.
    /// </summary>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A task that may or may not have succeeded.</returns>
    internal async Task FinishWalkingAsync(CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        IsFinished = true;
        _semaphore.Release();
        await CancelWalkingAsync(ct: ct);
    }

    private bool OnCharacterWalked(ushort x, ushort y)
    {
        if (IsWalking)
        {
            if (!_userCanCancel)
            {
                return false;
            }

            CancelWalkingAsync(WalkCancelReason.UserWalked)
                .GetAwaiter()
                .GetResult();
        }
        return true;
    }
}