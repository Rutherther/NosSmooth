//
//  MateStatEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Data.Mates;
using NosSmooth.Game.Data.Social;

namespace NosSmooth.Game.Events.Mates;

public record MateStatEvent(Mate Mate, Health Hp, Health Mp) : IGameEvent;