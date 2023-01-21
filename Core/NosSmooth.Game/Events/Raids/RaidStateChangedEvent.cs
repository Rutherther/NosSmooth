//
//  RaidStateChangedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Raids;

namespace NosSmooth.Game.Events.Raids;

/// <summary>
/// A raid <see cref="Raid"/> has changed a state from <see cref="PreviousState"/> to <see cref="CurrentState"/>.
/// </summary>
/// <param name="PreviousState">The previous state of the raid.</param>
/// <param name="CurrentState">The current state of the raid.</param>
/// <param name="Raid">The raid that has changed the state. The current raid, or the last one in case the raid was finished.</param>
public record RaidStateChangedEvent
(
    RaidState PreviousState,
    RaidState CurrentState,
    Raid Raid
) : IGameEvent;