//
//  ProcessNotFoundError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Injector.Errors;

/// <summary>
/// The given process was not found.
/// </summary>
/// <param name="ProcessId">The id of the process.</param>
public record ProcessNotFoundError(string ProcessId)
    : NotFoundError($"Could not find process with the given id {ProcessId}.");