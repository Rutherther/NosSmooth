//
//  PacketToken.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace NosSmooth.Packets;

/// <summary>
/// The single token from a packet.
/// </summary>
/// <param name="Token">The token.</param>
/// <param name="IsLast">Whether the token is last in the current level. Null if it cannot be determined.</param>
/// <param name="EncounteredUpperLevel">Whether the current separator was from an upper stack level than the parent. That could mean some kind of an error if not etc. at the end of parsing a last entry of a list and last entry of a subpacket.</param>
/// <param name="PacketEndReached">Whether the packet's end was reached.</param>
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Record struct creates the underlying properties.")]
public readonly record struct PacketToken(string Token, bool? IsLast, bool? EncounteredUpperLevel, bool PacketEndReached);