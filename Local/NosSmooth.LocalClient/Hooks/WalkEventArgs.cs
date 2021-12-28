//
//  WalkEventArgs.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.LocalClient.Hooks;

/// <summary>
/// The event args for event in <see cref="NostaleHookManager"/>.
/// </summary>
public class WalkEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WalkEventArgs"/> class.
    /// </summary>
    /// <param name="targetX">The target x coordinate.</param>
    /// <param name="targetY">The target y coordinate.</param>
    public WalkEventArgs(int targetX, int targetY)
    {
        TargetX = targetX;
        TargetY = targetY;
    }

    /// <summary>
    /// Gets the target x coordinate.
    /// </summary>
    public int TargetX { get; }

    /// <summary>
    /// Gets the target y coordinate.
    /// </summary>
    public int TargetY { get; }
}