//
//  RaidFinishedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Raids;

namespace NosSmooth.Game.Events.Raids;

public record RaidFinishedEvent(Raid Raid) : IGameEvent;