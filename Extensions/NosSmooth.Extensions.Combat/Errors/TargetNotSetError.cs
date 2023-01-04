//
//  TargetNotSetError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Extensions.Combat.Errors;

/// <summary>
/// The combat technique set target to null, the combat cannot continue.
/// </summary>
public record TargetNotSetError() : ResultError("The current target is not set, the operation cannot complete.");