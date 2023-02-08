//
//  MapChangedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Events.Map;

/// <summary>
/// A map has been changed.
/// </summary>
/// <param name="PreviousMap">The previous map.</param>
/// <param name="CurrentMap">The new map.</param>
public record MapChangedEvent
(
    Data.Maps.Map? PreviousMap,
    Data.Maps.Map CurrentMap
) : IGameEvent;