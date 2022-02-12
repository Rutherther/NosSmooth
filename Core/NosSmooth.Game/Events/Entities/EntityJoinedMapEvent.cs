//
//  EntityJoinedMapEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Entities;

namespace NosSmooth.Game.Events.Entities;

/// <summary>
/// The given entity has joined the map.
/// </summary>
/// <param name="Entity">The entity.</param>
public record EntityJoinedMapEvent(IEntity Entity) : IGameEvent;