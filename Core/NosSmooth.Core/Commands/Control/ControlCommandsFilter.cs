//
//  ControlCommandsFilter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace NosSmooth.Core.Commands.Control;

/// <summary>
/// Filter for cancellation of <see cref="TakeControlCommand"/>.
/// </summary>
[Flags]
public enum ControlCommandsFilter
{
    /// <summary>
    /// No filter, cancel all commands.
    /// </summary>
    None,

    /// <summary>
    /// Cancel commands that should be cancelled upon user action.
    /// </summary>
    UserCancellable,

    /// <summary>
    /// Cancel commands that should be cancelled upon map change.
    /// </summary>
    MapChangeCancellable
}