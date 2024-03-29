﻿//
//  Family.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Social;

/// <summary>
/// Represents nostale family entity.
/// </summary>
/// <param name="Id">The id of the family.</param>
/// <param name="Title">The family's title.</param>
/// <param name="Name">The name of the family.</param>
/// <param name="Level">The level of the entity.</param>
/// <param name="Icons">What icons the family has activated.</param>
public record Family
(
    long? Id,
    short? Title,
    string? Name,
    byte? Level,
    IReadOnlyList<bool>? Icons
);