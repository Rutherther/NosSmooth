//
//  EntityType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Packets.Enums.Entities;

/// <summary>
/// Type of the entity.
/// </summary>
public enum EntityType
{
    /// <summary>
    /// Unknown.
    /// </summary>
    Map = 0,

    /// <summary>
    /// The entity is a player.
    /// </summary>
    Player = 1,

    /// <summary>
    /// The entity is an npc. (can be pets and partners as well).
    /// </summary>
    Npc = 2,

    /// <summary>
    /// The entity is a monster.
    /// </summary>
    Monster = 3,

    /// <summary>
    /// The entity is an object, ie. gold or dropped item.
    /// </summary>
    Object = 9
}