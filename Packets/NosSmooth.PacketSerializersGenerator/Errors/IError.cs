//
//  IError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.PacketSerializersGenerator.Errors;

/// <summary>
/// Base error type.
/// </summary>
public interface IError
{
    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message { get; }
}