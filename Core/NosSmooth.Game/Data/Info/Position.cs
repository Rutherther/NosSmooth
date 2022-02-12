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
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313", MessageId = "Parameter names should begin with lower-case letter", Justification = "Standard.")]
public record struct Position(long X, long Y)
{
    /// <summary>
    /// Get the squared distance to the given position.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns>The distance squared.</returns>
    public long DistanceSquared(Position position)
    {
        return ((position.X - X) * (position.X - X)) + ((position.Y - Y) * (position.Y - Y));
    }
}