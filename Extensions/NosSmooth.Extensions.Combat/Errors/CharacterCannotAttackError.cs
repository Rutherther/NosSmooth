//
//  CharacterCannotAttackError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Extensions.Combat.Errors;

public record CharacterCannotAttackError()
    : ResultError("The character cannot currently attack (is stunned, under debuff)");