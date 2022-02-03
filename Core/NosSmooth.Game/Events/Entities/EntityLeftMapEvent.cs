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
public record EntityLeftMapEvent
(
    IEntity Entity,
    Portal? Portal,
    bool? Died
) : IGameEvent;