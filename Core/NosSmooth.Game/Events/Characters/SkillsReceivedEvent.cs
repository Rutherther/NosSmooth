//
//  SkillsReceivedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Characters;

namespace NosSmooth.Game.Events.Characters;

public record SkillsReceivedEvent(Skills Skills) : IGameEvent;