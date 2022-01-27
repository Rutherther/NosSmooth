//
//  ITakeControlCommand.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Core.Commands.Control;

/// <summary>
/// Represents command that supports taking control. See <see cref="TakeControlCommand"/>.
/// </summary>
public interface ITakeControlCommand : ICommand
{
    /// <summary>
    /// Gets whether the command may be cancelled by another task within the same group.
    /// </summary>
    bool CanBeCancelledByAnother { get; }

    /// <summary>
    /// Gets whether to wait for finish of the previous task.
    /// </summary>
    bool WaitForCancellation { get; }

    /// <summary>
    /// Whether to allow the user to cancel by taking any walk/focus/unfollow action.
    /// </summary>
    bool AllowUserCancel { get; }

    /// <summary>
    /// Whether the command should be cancelled on map change.
    /// </summary>
    bool CancelOnMapChange { get; }
}