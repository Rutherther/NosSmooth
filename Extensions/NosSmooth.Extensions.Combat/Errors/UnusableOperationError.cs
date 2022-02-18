//
//  UnusableOperationError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Extensions.Combat.Operations;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Errors;

/// <summary>
/// An error that tells the operation was unusable.
/// </summary>
/// <param name="Operation">The operation.</param>
public record UnusableOperationError(ICombatOperation Operation)
    : ResultError("A given operation {Operation} responded that it won't be usable ever and thus there is an unrecoverable state.");