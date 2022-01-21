//
//  IMapInfo.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Data.Abstractions.Infos;

/// <summary>
/// The NosTale map information.
/// </summary>
public interface IMapInfo : IVNumInfo
{
    /// <summary>
    /// Gets the width of the grid.
    /// </summary>
    public short Width { get; }

    /// <summary>
    /// Gets the height of the grid.
    /// </summary>
    public short Height { get; }

    /// <summary>
    /// Gets grid data for the given position.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns>The grid value.</returns>
    public byte GetData(short x, short y);

    /// <summary>
    /// Gets whether the given position is walkable.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns>Whether the position is walkable.</returns>
    public bool IsWalkable(short x, short y);
}