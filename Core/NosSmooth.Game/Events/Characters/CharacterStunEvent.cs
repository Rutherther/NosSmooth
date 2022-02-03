//
//  CharacterStunEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Events.Characters;

/// <summary>
/// The character has been stunned or unstunned.
/// </summary>
/// <param name="Stunned">Whether the character is stunned.</param>
public record CharacterStunEvent(bool Stunned);