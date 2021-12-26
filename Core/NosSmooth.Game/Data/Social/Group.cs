//
//  Group.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Entities;
using OneOf;

namespace NosSmooth.Game.Data.Social;

/// <summary>
/// Represents nostale group of players or pets and partners.
/// </summary>
/// <param name="Id">The id of the group.</param>
/// <param name="Size">The size of the group.</param>
/// <param name="Members">The members of the group. (excluding the character)</param>
public record Group(short? Id, byte? Size, IReadOnlyList<OneOf<Player, IPet>>? Members);