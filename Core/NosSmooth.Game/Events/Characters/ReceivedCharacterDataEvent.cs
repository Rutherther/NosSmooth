//
//  ReceivedCharacterDataEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Characters;

namespace NosSmooth.Game.Events.Characters;

/// <summary>
/// Represents received new updated character data.
/// </summary>
/// <param name="Character">The newly received data.</param>
public record ReceivedCharacterDataEvent
(
    Character Character
) : IGameEvent;