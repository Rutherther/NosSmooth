//
//  ControlCancelReason.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Core.Commands.Control;

/// <summary>
/// Reason for control cancellation.
/// </summary>
public enum ControlCancelReason
{
    /// <summary>
    /// Unknown reason for cancellation.
    /// </summary>
    Unknown,

    /// <summary>
    /// The user has took walk/unfollow action.
    /// </summary>
    UserAction,

    /// <summary>
    /// There was another task that cancelled this one.
    /// </summary>
    AnotherTask
}