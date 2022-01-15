//
//  PacketEndReachedError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.PacketSerializer.Abstractions.Errors;

/// <summary>
/// The end of a packet was reached already.
/// </summary>
/// <param name="Packet">The packet string.</param>
/// <param name="LevelEnd">Whether this indicates end of a level instead of the whole packet.</param>
public record PacketEndReachedError(string Packet, bool LevelEnd = false)
    : ResultError($"Reached and end of the packet (or it's level) {Packet}, cannot read any more tokens in the current level.");