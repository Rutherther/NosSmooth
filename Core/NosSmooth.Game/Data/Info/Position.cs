//
//  Position.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace NosSmooth.Game.Data.Info;

/// <summary>
/// Represents nostale position on map.
/// </summary>
[SuppressMessage
(
    "StyleCop.CSharp.NamingRules",
    "SA1313",
    MessageId = "Parameter names should begin with lower-case letter",
    Justification = "Standard."
)]
public record struct Position(short X, short Y)
{
    /// <summary>
    /// Gets the zero position.
    /// </summary>
    public static Position Zero => new Position(0, 0);

    /// <summary>
    /// Get the squared distance to the given position.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns>The distance squared.</returns>
    public long DistanceSquared(Position position)
    {
        return ((position.X - X) * (position.X - X)) + ((position.Y - Y) * (position.Y - Y));
    }

    /// <summary>
    /// Gets whether the given position is in the given range.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="range">The range.</param>
    /// <returns>Whether the position is in the range.</returns>
    public bool IsInRange(Position position, float range)
    {
        return DistanceSquared(position) <= range * range;
    }
    
    /// <summary>
    /// Multiply position.
    /// </summary>
    /// <param name="left">The left position.</param>
    /// <param name="right">The right position.</param>
    /// <returns>The multiplied position.</returns>
    public static Position operator *(short left, Position right)
    {
        return new Position((short)(left * right.X), (short)(left * right.Y));
    }

    /// <summary>
    /// Multiply position.
    /// </summary>
    /// <param name="left">The left position.</param>
    /// <param name="right">The right position.</param>
    /// <returns>The multiplied position.</returns>
    public static Position operator *(double left, Position right)
    {
        return new Position((short)(left * right.X), (short)(left * right.Y));
    }

    /// <summary>
    /// Add two positions.
    /// </summary>
    /// <param name="left">The left position.</param>
    /// <param name="right">The right position.</param>
    /// <returns>The added position.</returns>
    public static Position operator +(Position left, Position right)
    {
        return new Position((short)(left.X + right.X), (short)(left.Y + right.Y));
    }

    /// <summary>
    /// Subtract two positions.
    /// </summary>
    /// <param name="left">The left position.</param>
    /// <param name="right">The right position.</param>
    /// <returns>The subtracted position.</returns>
    public static Position operator -(Position left, Position right)
    {
        return new Position((short)(left.X - right.X), (short)(left.Y - right.Y));
    }
}