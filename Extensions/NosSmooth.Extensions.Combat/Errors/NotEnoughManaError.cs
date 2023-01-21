//
//  NotEnoughManaError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Extensions.Combat.Errors;

public record NotEnoughManaError(long CurrentMana, long NeededMana)
    : ResultError($"The character (with {CurrentMana} mp) does not have enough mana ({NeededMana} mp) for the given operation.");