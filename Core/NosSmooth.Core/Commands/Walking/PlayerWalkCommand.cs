//
//  PlayerWalkCommand.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Commands.Control;

namespace NosSmooth.Core.Commands.Walking;

/// <summary>
/// Command that moves the player to the specified target position.
/// May be used only in world.
/// </summary>
/// <param name="TargetX">The x coordinate of the target position to move to.</param>
/// <param name="TargetY">The y coordinate of the target position to move to.</param>
/// <param name="CanBeCancelledByAnother">Whether the command may be cancelled by another task within the same group.</param>
/// <param name="WaitForCancellation">Whether to wait for finish of the previous task</param>
/// <param name="AllowUserCancel">Whether to allow the user to cancel by taking any walk/focus/unfollow action</param>
public record PlayerWalkCommand
(
    short TargetX,
    short TargetY,
    bool CanBeCancelledByAnother = true,
    bool WaitForCancellation = true,
    bool AllowUserCancel = true
) : ITakeControlCommand
{
    /// <inheritdoc />
    public bool CancelOnMapChange => true;
}