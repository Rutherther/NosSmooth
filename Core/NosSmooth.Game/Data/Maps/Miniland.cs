//
//  Miniland.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Infos;

namespace NosSmooth.Game.Data.Maps;

/// <summary>
/// Represents Miniland map that can contain miniland objects.
/// </summary>
/// <param name="Id">The id of the miniland map.</param>
/// <param name="Type">The type of the miniland map.</param>
/// <param name="Info">The information about the map obtained from data assembly.</param>
/// <param name="Entities">The entities on the map.</param>
/// <param name="Portals">The portals on the map.</param>
/// <param name="Objects">The miniland objects on the map.</param>
public record Miniland
(
    long Id,
    byte Type,
    IMapInfo? Info,
    MapEntities Entities,
    IReadOnlyList<Portal> Portals,
    IReadOnlyList<MinilandObject>? Objects
) : Map
(
    Id,
    Type,
    Info,
    Entities,
    Portals
);