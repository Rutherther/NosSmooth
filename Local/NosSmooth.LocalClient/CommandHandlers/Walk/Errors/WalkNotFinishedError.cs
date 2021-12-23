//
//  WalkNotFinishedError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.LocalClient.CommandHandlers.Walk.Errors;

/// <summary>
/// Represents an error that can be returned from walk command handler.
/// </summary>
/// <param name="X">The x coordinate where the player is. (if known)</param>
/// <param name="Y">The y coordinate where the player is. (if known)</param>
/// <param name="Reason"></param>
public record WalkNotFinishedError(int? X, int? Y, WalkCancelReason Reason)
    : ResultError($"Could not finish the walk to {X} {Y}, because {Reason}");