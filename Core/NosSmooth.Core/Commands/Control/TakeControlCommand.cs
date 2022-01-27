﻿//
//  TakeControlCommand.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Remora.Results;

namespace NosSmooth.Core.Commands.Control;

/// <summary>
/// Take control in the given group and call the given callback with a cancellation token.
/// </summary>
/// <remarks>
/// The command will be cancelled either if the program tries to take control in the same group again
/// or the user has clicked somewhere to cancel the operation.
/// </remarks>
/// <param name="HandleCallback">The callback to be called when control is granted.</param>
/// <param name="CancelledCallback">The callback to be called if the operation was cancelled and the control was revoked.</param>
/// <param name="Group">The group of the take control.</param>
public record TakeControlCommand
(
    Func<CancellationToken, Task<Result>> HandleCallback,
    Func<ControlCancelReason, Task<Result>> CancelledCallback,
    string Group = "__default",
    bool CanBeCancelledByAnother = true,
    bool WaitForCancellation = true,
    bool AllowUserCancel = true,
    bool CancelOnMapChange = true
) : ITakeControlCommand;