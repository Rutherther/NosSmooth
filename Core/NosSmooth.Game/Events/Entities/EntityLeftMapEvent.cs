//
//  EntityLeftMapEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Maps;

namespace NosSmooth.Game.Events.Entities;

/// <summary>
/// An entity has left the map.
/// </summary>
/// <param name="Entity">The entity that has left.</param>
/// <param name="Portal">The portal that was probably used to leave the map. (This may not be accurate, the character may as well have left or )</param>
/// <param name="Died">Whether the entity has died. (This may not be accurate.)</param>
public record EntityLeftMapEvent
(
    IEntity Entity,
    Portal? Portal,
    bool? Died
) : IGameEvent;