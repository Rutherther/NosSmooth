//
//  MovedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;

namespace NosSmooth.Game.Events.Entities;

public record MovedEvent
(
    IEntity Entity,
    long EntityId,
    Position? OldPosition,
    Position NewPosition
) : IGameEvent;