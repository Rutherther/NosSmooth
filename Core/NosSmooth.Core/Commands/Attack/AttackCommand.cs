//
//  AttackCommand.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Commands.Control;
using Remora.Results;

namespace NosSmooth.Core.Commands.Attack;

/// <summary>
/// A take control command used to take control of attacking at a given target.
/// </summary>
/// <param name="TargetId">The id of the target the bot will be attacking.</param>
/// <param name="HandleAttackCallback">The callback to be called when the control has been gained.</param>
/// <param name="CanBeCancelledByAnother">Whether the command may be cancelled by another task within the same group.</param>
/// <param name="WaitForCancellation">Whether to wait for finish of the previous task</param>
/// <param name="AllowUserCancel">Whether to allow the user to cancel by taking any walk/focus/unfollow action</param>
public record AttackCommand
(
    long? TargetId,
    Func<CancellationToken, Task<Result>> HandleAttackCallback,
    bool CanBeCancelledByAnother = true,
    bool WaitForCancellation = true,
    bool AllowUserCancel = true
) : ITakeControlCommand
{
    /// <inheritdoc />
    public bool CancelOnMapChange => true;
}