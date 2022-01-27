//
//  CouldNotGainControlError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Core.Errors;

/// <summary>
/// Could not gain control of the given group.
/// </summary>
/// <param name="Group">The group name.</param>
/// <param name="Message">The message.</param>
public record CouldNotGainControlError(string Group, string Message)
    : ResultError($"Could not cancel an operation from {Group} due to {Message}");