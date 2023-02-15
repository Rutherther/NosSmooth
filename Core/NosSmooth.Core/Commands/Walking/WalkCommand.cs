//
//  WalkCommand.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using NosSmooth.Core.Commands.Control;

namespace NosSmooth.Core.Commands.Walking;

/// <summary>
/// Walk player and pets simultaneously.
/// </summary>
/// <param name="TargetX">The target x coordinate.</param>
/// <param name="TargetY">The target y coordinate.</param>
/// <param name="Pets">The pet walk positions.</param>
/// <param name="ReturnDistanceTolerance">The distance tolerance to the target when to return successful result.</param>
/// <param name="CanBeCancelledByAnother">Whether the command may be cancelled by another task within the same group.</param>
/// <param name="WaitForCancellation">Whether to wait for finish of the previous task</param>
/// <param name="AllowUserCancel">Whether to allow the user to cancel by taking any walk/focus/unfollow action</param>
public record WalkCommand
(
    short TargetX,
    short TargetY,
    ushort ReturnDistanceTolerance,
    IReadOnlyList<(long MateId, short TargetX, short TargetY)>? Pets = default,
    bool CanBeCancelledByAnother = true,
    bool WaitForCancellation = true,
    bool AllowUserCancel = true
) : ITakeControlCommand
{
    /// <inheritdoc />
    public bool CancelOnMapChange => true;
}