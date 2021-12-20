//
//  CommandHandlerNotFound.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Remora.Results;

namespace NosSmooth.Core.Errors;

/// <summary>
/// Represents an error that tells the user there is no handler for the specified command so it cannot be processed.
/// </summary>
/// <param name="CommandType">The type of the command.</param>
public record CommandHandlerNotFound(Type CommandType) : ResultError(
    $"Could not process the command of type {CommandType.FullName}, because there is not a handler for it.");