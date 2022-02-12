//
//  SkillsReceivedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Characters;

namespace NosSmooth.Game.Events.Characters;

/// <summary>
/// Received skills of the character.
/// </summary>
/// <param name="Skills">The skills.</param>
public record SkillsReceivedEvent(Skills Skills) : IGameEvent;