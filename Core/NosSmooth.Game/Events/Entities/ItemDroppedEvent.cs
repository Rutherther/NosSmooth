//
//  ItemDroppedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Entities;

namespace NosSmooth.Game.Events.Entities;

/// <summary>
/// An item has been dropped.
/// </summary>
/// <param name="Item">The item that has been dropped.</param>
public record ItemDroppedEvent(GroundItem Item) : IGameEvent;