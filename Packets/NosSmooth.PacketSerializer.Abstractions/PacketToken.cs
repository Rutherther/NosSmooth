//
//  PacketToken.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace NosSmooth.PacketSerializer.Abstractions;

/// <summary>
/// The single token from a packet.
/// </summary>
[SuppressMessage
(
    "StyleCop.CSharp.NamingRules",
    "SA1313:Parameter names should begin with lower-case letter",
    Justification = "Record struct creates the underlying properties."
)]
public readonly ref struct PacketToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PacketToken"/> struct.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="isLast">Whether this is the last token in the current level.</param>
    /// <param name="encounteredUpperLevel">Whether upper level separator was encountered.</param>
    /// <param name="packetEndReached">Whether the packet end was reached.</param>
    public PacketToken
    (
        ReadOnlySpan<char> token,
        bool? isLast,
        bool? encounteredUpperLevel,
        bool packetEndReached
    )
    {
        Token = token;
        IsLast = isLast;
        EncounteredUpperLevel = encounteredUpperLevel;
        PacketEndReached = packetEndReached;
    }

    /// <summary>
    /// The token.
    /// </summary>
    public ReadOnlySpan<char> Token { get; }

    /// <summary>
    /// Whether the token is last in the current level. Null if it cannot be determined.
    /// </summary>
    public bool? IsLast { get; }

    /// <summary>
    /// Whether the current separator was from an upper stack level than the parent. That could mean some kind of an error if not etc. at the end of parsing a last entry of a list and last entry of a subpacket.
    /// </summary>
    public bool? EncounteredUpperLevel { get; }

    /// <summary>
    /// Whether the packet's end was reached.
    /// </summary>
    public bool PacketEndReached { get; }
}