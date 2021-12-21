//
//  ChatMessage.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Entities;

namespace NosSmooth.Game.Data.Chat;

/// <summary>
/// Message received from user in chat.
/// </summary>
/// <param name="CharacterId">The id of the character.</param>
/// <param name="Player">The player </param>
/// <param name="Message">The message sent from the friend.</param>
public record ChatMessage(long CharacterId, Player? Player, string Message);