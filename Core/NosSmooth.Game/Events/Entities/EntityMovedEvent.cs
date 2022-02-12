//
//  EntityMovedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;

namespace NosSmooth.Game.Events.Entities;

/// <summary>
/// An entity has moved.
/// </summary>
/// <param name="Entity">The entity.</param>
/// <param name="OldPosition">The previous position of the entity.</param>
/// <param name="NewPosition">The new position of the entity.</param>
public record EntityMovedEvent
(
    IEntity Entity,
    Position? OldPosition,
    Position NewPosition
) : IGameEvent;