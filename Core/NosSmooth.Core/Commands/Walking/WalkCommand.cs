//
//  WalkCommand.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Commands.Control;

namespace NosSmooth.Core.Commands.Walking;

/// <summary>
/// Walk player and pets simultaneously.
/// </summary>
/// <param name="TargetX">The target x coordinate.</param>
/// <param name="TargetY">The target y coordinate.</param>
/// <param name="PetSelectors">The pet indices.</param>
/// <param name="CanBeCancelledByAnother">Whether the command may be cancelled by another task within the same group.</param>
/// <param name="WaitForCancellation">Whether to wait for finish of the previous task</param>
/// <param name="AllowUserCancel">Whether to allow the user to cancel by taking any walk/focus/unfollow action</param>
public record WalkCommand
(
    short TargetX,
    short TargetY,
    int[] PetSelectors,
    bool CanBeCancelledByAnother = true,
    bool WaitForCancellation = true,
    bool AllowUserCancel = true
) : ITakeControlCommand
{
    /// <inheritdoc />
    public bool CancelOnMapChange => true;
}