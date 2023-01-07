//
//  MatesPartyInitializedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Mates;
using NosSmooth.Game.Data.Social;

namespace NosSmooth.Game.Events.Mates;

/// <summary>
/// A party was initialized and got information about currently present party pets.
/// </summary>
/// <param name="Pet">The party pet.</param>
/// <param name="Partner">The party partner.</param>
public record MatesPartyInitializedEvent(Pet? Pet, Partner? Partner) : IGameEvent;