//
//  WrongTypeError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.PacketSerializer.Abstractions.Errors;

/// <summary>
/// The wrong type was passed to a type converter.
/// </summary>
/// <param name="TypeConverter">The converter that failed to convert the object.</param>
/// <param name="ExpectedType">The expected type of the converting object.</param>
/// <param name="ActualObject">The actual object the converter got.</param>
public record WrongTypeError(IStringConverter TypeConverter, Type ExpectedType, object? ActualObject)
    : ResultError($"{TypeConverter.GetType().FullName} expected type {ExpectedType.FullName}, but got {ActualObject?.GetType().FullName ?? "null"}");