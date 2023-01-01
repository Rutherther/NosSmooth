//
//  PacketConverterNotFoundError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.PacketSerializer.Errors;

/// <summary>
/// The converter for the given packet was not found.
/// </summary>
/// <param name="Header">The header of the packet.</param>
public record PacketConverterNotFoundError(string Header)
    : ResultError($"Could not find converter for packet with header {Header}.");