//
//  Pathfinder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Extensions.Pathfinding.Errors;
using NosSmooth.Packets.Enums;
using Remora.Results;

namespace NosSmooth.Extensions.Pathfinding;

/// <summary>
/// Find path between two given points.
/// </summary>
public class Pathfinder
{
    private readonly PathfinderState _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="Pathfinder"/> class.
    /// </summary>
    /// <param name="state">The state.</param>
    public Pathfinder(PathfinderState state)
    {
        _state = state;
    }

    /// <summary>
    /// Attempts to find a path between the current position and the target.
    /// </summary>
    /// <param name="targetX">The target x coordinate.</param>
    /// <param name="targetY">The target y coordinate.</param>
    /// <returns>A path or an error.</returns>
    public Result<Path> FindPathFromCurrent
    (
        short targetX,
        short targetY
    )
        => FindPathFrom(_state.X, _state.Y, targetX, targetY);

    /// <summary>
    /// Attempts to find a path between the given positions on the current map.
    /// </summary>
    /// <param name="x">The start x coordinate.</param>
    /// <param name="y">The start y coordinate.</param>
    /// <param name="targetX">The target x coordinate.</param>
    /// <param name="targetY">The target y coordinate.</param>
    /// <returns>A path or an error.</returns>
    public Result<Path> FindPathFrom
    (
        short x,
        short y,
        short targetX,
        short targetY
    )
    {
        if (_state.MapInfo is null)
        {
            return new StateNotInitializedError();
        }

        return FindPathOnMap
        (
            _state.MapInfo,
            x,
            y,
            targetX,
            targetY
        );
    }

    /// <summary>
    /// Attempts to find path on the given map with the given coordinates.
    /// </summary>
    /// <param name="mapInfo">The map info.</param>
    /// <param name="x">The start x coordinate.</param>
    /// <param name="y">The start y coordinate.</param>
    /// <param name="targetX">The target x coordinate.</param>
    /// <param name="targetY">The target y coordinate.</param>
    /// <returns>A path or an error.</returns>
    public Result<Path> FindPathOnMap
    (
        IMapInfo mapInfo,
        short x,
        short y,
        short targetX,
        short targetY
    )
    {
        if (!mapInfo.IsWalkable(targetX, targetY))
        {
            return new NotFoundError("The requested target is not walkable, path cannot be found.");
        }

        if (x == targetX && y == targetY)
        {
            return new Path(mapInfo.Id, x, y, targetX, targetY, Array.Empty<(short X, short Y)>());
        }

        var target = (targetX, targetY);
        var visited = new HashSet<(short X, short Y)>();
        var offsets = new (short X, short Y)[] { (0, 1), (1, 0), (1, 1), (0, -1), (-1, 0), (-1, -1), (1, -1), (-1, 1) };
        var distances = new[] { 1, 1, 1.41421356237, 1, 1, 1.41421356237, 1.41421356237, 1.41421356237 };
        var queue = new PriorityQueue<PathEntry, double>(); // estimated cost to path.

        var start = new PathEntry(0, null, (x, y));
        queue.Enqueue(start, 0);
        visited.Add((x, y));

        while (queue.TryDequeue(out var current, out _))
        {
            for (int i = 0; i < offsets.Length; i++)
            {
                var offset = offsets[i];
                var distance = distances[i];

                var currX = current.Position.X + offset.X;
                var currY = current.Position.Y + offset.Y;

                if (visited.Contains(((short)currX, (short)currY)))
                {
                    // The estimated distance function should be consistent,
                    // the cost cannot be lower on this visit.
                    continue;
                }
                visited.Add(((short)currX, (short)currY));

                if (currX == targetX && currY == targetY)
                {
                    return ReconstructPath
                    (
                        mapInfo.Id,
                        x,
                        y,
                        targetX,
                        targetY,
                        current.CreateChild(distance, (short)currX, (short)currY)
                    );
                }

                if (currX < 0 || currY < 0 || currX >= mapInfo.Width || currY >= mapInfo.Height)
                {
                    // Out of bounds
                    continue;
                }

                if (!mapInfo.IsWalkable((short)currX, (short)currY))
                {
                    // Current tile not walkable
                    continue;
                }

                var path = current.CreateChild(distance, (short)currX, (short)currY);
                var estimatedDistance = EstimateDistance(path.Position, target);
                queue.Enqueue(path, path.Cost + estimatedDistance);
            }
        }

        return new PathNotFoundError(targetX, targetY);
    }

    private Path ReconstructPath
    (
        int mapId,
        short x,
        short y,
        short targetX,
        short targetY,
        PathEntry entry
    )
    {
        var entries = new List<(short X, short Y)>();
        var current = entry;
        while (current is not null)
        {
            entries.Add(current.Position);
            current = current.Previous;
        }

        entries.Reverse();
        return new Path
        (
            mapId,
            x,
            y,
            targetX,
            targetY,
            entries
        );
    }

    private double EstimateDistance((short X, short Y) current, (short X, short Y) next)
    {
        return Math.Sqrt(((current.X - next.X) * (current.X - next.X)) + ((current.Y - next.Y) * (current.Y - next.Y)));
    }

    private class PathEntry
    {
        public PathEntry(double cost, PathEntry? previous, (short X, short Y) position)
        {
            Cost = cost;
            Previous = previous;
            Position = position;
        }

        public double Cost { get; }

        public PathEntry? Previous { get; }

        public (short X, short Y) Position { get; }

        public PathEntry CreateChild(double walkCost, short x, short y)
        {
            return new PathEntry(Cost + walkCost, this, (x, y));
        }
    }
}