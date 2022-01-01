//
//  WrongTypeError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.Packets.Converters;
using Remora.Results;

namespace NosSmooth.Packets.Errors;

/// <summary>
/// The wrong type was passed to a type converter.
/// </summary>
/// <param name="TypeConverter">The converter that failed to convert the object.</param>
/// <param name="ExpectedType">The expected type of the converting object.</param>
/// <param name="ActualObject">The actual object the converter got.</param>
public record WrongTypeError(ITypeConverter TypeConverter, Type ExpectedType, object? ActualObject)
    : ResultError($"{TypeConverter.GetType().FullName} expected type {ExpectedType.FullName}, but got {ActualObject?.GetType().FullName ?? "null"}");