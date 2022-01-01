//
//  UnresolvedPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Packets.Packets;

/// <summary>
/// Unresolved packet that is not supported.
/// </summary>
/// <param name="Header">The header of the packet.</param>
/// <param name="Body">The body of the packet.</param>
public record UnresolvedPacket(string Header, string Body) : IPacket;