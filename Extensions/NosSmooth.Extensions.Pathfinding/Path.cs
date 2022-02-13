//
//  Path.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Extensions.Pathfinding;

/// <summary>
/// Represents a found walkable path.
/// </summary>
public class Path
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Path"/> class.
    /// </summary>
    /// <param name="map">The map id.</param>
    /// <param name="x">The current x.</param>
    /// <param name="y">The current y.</param>
    /// <param name="targetX">The target x.</param>
    /// <param name="targetY">The target y.</param>
    /// <param name="parts">The parts that represent the path from the current to the target.</param>
    public Path
    (
        int map,
        short x,
        short y,
        short targetX,
        short targetY,
        IReadOnlyList<(short X, short Y)> parts
    )
    {
        MapId = map;
        CurrentX = x;
        CurrentY = y;
        TargetX = targetX;
        TargetY = targetY;
        Parts = parts;
        CurrentPartIndex = 0;
    }

    /// <summary>
    /// Gets the map id this path is for.
    /// </summary>
    public int MapId { get; }

    /// <summary>
    /// Gets whether the path has reached an end.
    /// </summary>
    public bool ReachedEnd => CurrentPartIndex >= Parts.Count - 1;

    /// <summary>
    /// Gets the current walk path index.
    /// </summary>
    public int CurrentPartIndex { get; private set; }

    /// <summary>
    /// Gets the list of the parts that have to be taken.
    /// </summary>
    public IReadOnlyList<(short X, short Y)> Parts { get; }

    /// <summary>
    /// Gets the target x coordinate.
    /// </summary>
    public short TargetX { get; }

    /// <summary>
    /// Gets the target y coordinate.
    /// </summary>
    public short TargetY { get; }

    /// <summary>
    /// Gets the current x coordinate.
    /// </summary>
    public short CurrentX { get; private set; }

    /// <summary>
    /// gets the current y coordinate.
    /// </summary>
    public short CurrentY { get; private set; }

    /// <summary>
    /// Take a path only in the same direction.
    /// </summary>
    /// <returns>A position to walk to.</returns>
    public (short X, short Y) TakeForwardPath()
    {
        if (ReachedEnd || CurrentPartIndex + 2 >= Parts.Count)
        {
            return (TargetX, TargetY);
        }

        var zeroTile = (CurrentX, CurrentY);
        var firstTile = Parts[++CurrentPartIndex];
        var currentTile = firstTile;
        var nextTile = Parts[CurrentPartIndex + 1];

        while (!ReachedEnd && IsInLine(zeroTile, firstTile, nextTile))
        {
            currentTile = nextTile;
            CurrentPartIndex++;
            if (!ReachedEnd)
            {
                nextTile = Parts[CurrentPartIndex + 1];
            }
        }

        return currentTile;
    }

    private bool IsInLine((short X, short Y) start, (short X, short Y) first, (short X, short Y) current)
    {
        var xFirstDiff = first.X - start.X;
        var yFirstDiff = first.Y - start.Y;

        var xCurrentDiff = current.X - start.X;
        var yCurrentDiff = current.Y - start.Y;

        if (xFirstDiff == 0 && yFirstDiff == 0)
        {
            throw new ArgumentException("The path went back to the start.");
        }

        if (xCurrentDiff == 0 && yCurrentDiff == 0)
        {
            throw new ArgumentException("The path went back to the start.");
        }

        if (xFirstDiff != 0)
        {
            var xRatio = xCurrentDiff / (float)xFirstDiff;
            return (yFirstDiff * xRatio) - yCurrentDiff < float.Epsilon * 10;
        }

        var yRatio = yCurrentDiff / (float)yFirstDiff;
        return (xFirstDiff * yRatio) - xCurrentDiff < float.Epsilon * 10;
    }

    /// <summary>
    /// Take the given number of tiles and return the position we ended up at.
    /// </summary>
    /// <remarks>
    /// If the count is greater than what is remaining, the end will be taken.
    /// </remarks>
    /// <param name="partsCount">The count of parts to take.</param>
    /// <returns>A position to walk to.</returns>
    public (short X, short Y) TakePath(uint partsCount)
    {
        if (ReachedEnd)
        {
            return (TargetX, TargetY);
        }

        if (CurrentPartIndex + partsCount >= Parts.Count - 1)
        {
            CurrentPartIndex = Parts.Count - 1;
        }
        else
        {
            CurrentPartIndex += (int)partsCount;
        }

        return Parts[CurrentPartIndex];
    }
}