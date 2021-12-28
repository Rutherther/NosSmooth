//
//  TypeConverterNotFoundError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Remora.Results;

namespace NosSmooth.Packets.Errors;

/// <summary>
/// Could not find type converter for the given type.
/// </summary>
/// <param name="Type">The type of the object.</param>
public record TypeConverterNotFoundError(Type Type) : ResultError($"Could not find converter for {Type.FullName}.");