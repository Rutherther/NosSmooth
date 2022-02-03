//
//  CharacterDiedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Characters;

namespace NosSmooth.Game.Events.Characters;

/// <summary>
/// The playing character has died.
/// </summary>
public record CharacterDiedEvent(Skill? KillSkill) : IGameEvent;