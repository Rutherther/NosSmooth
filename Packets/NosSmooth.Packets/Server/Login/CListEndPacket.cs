//
//  CListEndPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Login;

/// <summary>
/// End of character list.
/// </summary>
/// <remarks>
/// Before clist, clist_start will be sent.
/// clist for each character will follow,
/// after that, clist_end will be sent.
/// </remarks>
[GenerateSerializer(true)]
[PacketHeader("clist_end", PacketSource.Server)]
public record CListEndPacket() : IPacket;