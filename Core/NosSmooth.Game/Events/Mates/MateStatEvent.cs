//
//  MateStatEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Data.Mates;
using NosSmooth.Game.Data.Social;

namespace NosSmooth.Game.Events.Mates;

/// <summary>
/// A new stats (hp, mp) of a mate received.
/// </summary>
/// <param name="Mate">The mate.</param>
/// <param name="Hp">The current hp of the mate.</param>
/// <param name="Mp">The current mp of the mate.</param>
public record MateStatEvent(Mate Mate, Health Hp, Health Mp) : IGameEvent;