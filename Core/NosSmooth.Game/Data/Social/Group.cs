//
//  Group.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Maps;
using NosSmooth.Packets.Enums.Entities;
using OneOf;
using Remora.Results;

namespace NosSmooth.Game.Data.Social;

/// <summary>
/// Represents nostale group of players or pets and partners.
/// </summary>
/// <param name="Id">The id of the group.</param>
/// <param name="Members">The members of the group. (excluding the character)</param>
public record Group(short? Id, IReadOnlyList<GroupMember>? Members)
{
    /// <summary>
    /// Gets the living entities from the map associated with group members.
    /// </summary>
    /// <remarks>
    /// If a group member is not found on the map, null will be returned instead on its position.
    /// </remarks>
    /// <param name="game">The map to get map from.</param>
    /// <returns>The living entities representing group members.</returns>
    public IReadOnlyList<Player?> GetLivingEntities(Game game)
        => GetLivingEntities(game.CurrentMap);

    /// <summary>
    /// Gets the living entities from the map associated with group members.
    /// </summary>
    /// <remarks>
    /// If a group member is not found on the map, null will be returned instead on its position.
    /// </remarks>
    /// <param name="map">The map to get entities from.</param>
    /// <returns>The living entities representing group members.</returns>
    public IReadOnlyList<Player?> GetLivingEntities(Map? map)
        => GetLivingEntities(map?.Entities);

    /// <summary>
    /// Gets the living entities from the map associated with group members.
    /// </summary>
    /// <remarks>
    /// If a group member is not found on the map, null will be returned instead on its position.
    /// </remarks>
    /// <param name="entities">The entities to look at.</param>
    /// <returns>The living entities representing group members.</returns>
    public IReadOnlyList<Player?> GetLivingEntities(MapEntities? entities)
    {
        return (IReadOnlyList<Player?>?)Members?
            .Select(x => entities?.GetEntity<Player>(x.PlayerId))
            .ToList() ?? Array.Empty<Player?>();
    }
}