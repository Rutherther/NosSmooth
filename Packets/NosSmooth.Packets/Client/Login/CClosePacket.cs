//
//  CClosePacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client.Login;

/// <summary>
/// Unknown function
/// </summary>
/// <param name="Unknown">Unknown, seems to be either 0 or 1.</param>
[GenerateSerializer(true)]
[PacketHeader("c_close", PacketSource.Client)]
public record CClosePacket
(
    [PacketIndex(0)]
    byte Unknown
) : IPacket;