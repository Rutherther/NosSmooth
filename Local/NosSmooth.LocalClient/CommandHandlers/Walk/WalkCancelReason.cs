//
//  WalkCancelReason.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Commands;

namespace NosSmooth.LocalClient.CommandHandlers.Walk;

/// <summary>
/// Reason of cancellation of <see cref="WalkCommand"/>.
/// </summary>
public enum WalkCancelReason
{
    /// <summary>
    /// There was an unknown cancel reason.
    /// </summary>
    Unknown,

    /// <summary>
    /// The user walked and CancelOnUserMove flag was set.
    /// </summary>
    UserWalked,

    /// <summary>
    /// The map has changed during the walk was in progress.
    /// </summary>
    MapChanged,

    /// <summary>
    /// The client was not walking for too long.
    /// </summary>
    NotWalkingTooLong,

    /// <summary>
    /// The nostale walk function has returned false.
    /// </summary>
    /// <remarks>
    /// The player may be stunned or immobile.
    /// </remarks>
    NosTaleReturnedFalse
}