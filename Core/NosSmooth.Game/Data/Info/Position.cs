//
//  Position.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Info;

/// <summary>
/// Represents nostale position on map.
/// </summary>
public record Position
{
    /// <summary>
    /// Gets the x coordinate.
    /// </summary>
    public long X { get; internal set; }

    /// <summary>
    /// Gets the y coordinate.
    /// </summary>
    public long Y { get; internal set; }
}