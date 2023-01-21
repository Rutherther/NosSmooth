//
//  CannotBeUsedError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Extensions.Combat.Operations;

public record CannotBeUsedError(CanBeUsedResponse Response, IResultError? UnderlyingError)
    : ResultError($"The given operation cannot move forward ({Response}). {UnderlyingError?.Message}");