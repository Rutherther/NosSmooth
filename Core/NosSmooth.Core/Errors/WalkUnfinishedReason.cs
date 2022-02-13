//
//  WalkUnfinishedReason.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Core.Errors;

/// <summary>
/// Reason for not finishing a walk.
/// </summary>
public enum WalkUnfinishedReason
{
    /// <summary>
    /// There was an unknown unfinished reason.
    /// </summary>
    Unknown,

    /// <summary>
    /// The client could not find path to the given location.
    /// </summary>
    /// <remarks>
    /// The user walked just some part of the path.
    /// </remarks>
    PathNotFound,

    /// <summary>
    /// The user has took an action that has cancelled the walk.
    /// </summary>
    UserAction,

    /// <summary>
    /// There was another walk action that cancelled this one.
    /// </summary>
    AnotherTask
}