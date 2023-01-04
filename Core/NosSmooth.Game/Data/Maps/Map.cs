//
//  Map.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Game.Data.Info;

namespace NosSmooth.Game.Data.Maps;

/// <summary>
/// Represents nostale map.
/// </summary>
/// <param name="Id">The id of the map.</param>
/// <param name="Type">The type of the map.</param>
/// <param name="Info">The information about the map obtained from data assembly.</param>
/// <param name="Entities">The entities on the map.</param>
/// <param name="Portals">The portals on the map.</param>
public record Map
(
    long Id,
    byte Type,
    IMapInfo? Info,
    MapEntities Entities,
    IReadOnlyList<Portal> Portals
)
{
    /// <summary>
    /// Gets whether the given position lies on a portal.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="portal">The portal the position is on, if any.</param>
    /// <returns>Whether there was a portal at the specified position.</returns>
    public bool IsOnPortal(Position position, [NotNullWhen(true)] out Portal? portal)
    {
        foreach (var p in Portals)
        {
            // TODO: figure out the distance
            if (p.Position.DistanceSquared(position) < 3)
            {
                portal = p;
                return true;
            }
        }

        portal = null;
        return false;
    }
}