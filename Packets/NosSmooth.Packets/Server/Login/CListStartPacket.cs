//
//  CListStartPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Server.Groups;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Login;

/// <summary>
/// Signals start of character list.
/// </summary>
/// <remarks>
/// clist for each character will follow,
/// after that clist_end will be sent.
/// </remarks>
/// <param name="Unknown">Unknown, seems to be always 0.</param>
[GenerateSerializer(true)]
[PacketHeader("clist_start", PacketSource.Server)]
public record CListStartPacket
(
    [PacketIndex(0)]
    int Unknown = 0
) : IPacket;