//
//  TakeControlCommandExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Commands.Control;
using Remora.Results;

namespace NosSmooth.Core.Extensions;

/// <summary>
/// Extension methods for <see cref="ITakeControlCommand"/>.
/// </summary>
public static class TakeControlCommandExtensions
{
    /// <summary>
    /// Create a take control command base on a command that supports taking control over.
    /// </summary>
    /// <param name="takeControlCommand">The command implementing take control to copy.</param>
    /// <param name="group">The group of the new take control.</param>
    /// <param name="handleCallback">The callback to be called when control is granted.</param>
    /// <param name="cancellationCallback">The callback to be called if the operation was cancelled and the control was revoked.</param>
    /// <returns>The copied take control command.</returns>
    public static TakeControlCommand CreateTakeControl
    (
        this ITakeControlCommand takeControlCommand,
        string group,
        Func<CancellationToken, Task<Result>> handleCallback,
        Func<ControlCancelReason, Task<Result>> cancellationCallback
    )
    {
        return new TakeControlCommand
        (
            handleCallback,
            cancellationCallback,
            group,
            takeControlCommand.CanBeCancelledByAnother,
            takeControlCommand.WaitForCancellation,
            takeControlCommand.AllowUserCancel,
            takeControlCommand.CancelOnMapChange
        );
    }
}