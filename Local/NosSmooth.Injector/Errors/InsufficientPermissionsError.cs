//
//  InsufficientPermissionsError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Injector.Errors;

/// <summary>
/// The current user has insufficient permissions to inject into the given process.
/// </summary>
/// <param name="ProcessId">The id of the process.</param>
/// <param name="ProcessName">The name of the process.</param>
public record InsufficientPermissionsError(long ProcessId, string ProcessName)
    : ResultError
    (
        $"Insufficient permissions to open process {ProcessId} ({ProcessName}). Try running the injector as administrator."
    );