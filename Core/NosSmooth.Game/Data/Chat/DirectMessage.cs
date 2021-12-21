//
//  DirectMessage.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Chat;

/// <summary>
/// Message received from a friend.
/// </summary>
/// <param name="CharacterId">The id of the character.</param>
/// <param name="Friend">The friend from which the message is. May be null if the client did not receive friend packet.</param>
/// <param name="Message">The message sent from the friend.</param>
public record DirectMessage(long CharacterId, Friend? Friend, string? Message);