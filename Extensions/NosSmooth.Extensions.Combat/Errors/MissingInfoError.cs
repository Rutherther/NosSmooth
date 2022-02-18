//
//  MissingInfoError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Extensions.Combat.Errors;

/// <summary>
/// Missing info of a specified type and id..
/// </summary>
/// <param name="Type">The type.</param>
/// <param name="Id">The id.</param>
public record MissingInfoError(string Type, long Id) : ResultError($"Cannot find info of {{Type}} with id {Id}.");