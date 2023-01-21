//
//  CharacterCannotMoveError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Extensions.Combat.Errors;

public record CharacterCannotMoveError()
    : ResultError("The character cannot currently move (is stunned, under debuff)");