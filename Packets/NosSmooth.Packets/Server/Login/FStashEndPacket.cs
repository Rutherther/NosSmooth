//
//  FStashEndPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Login;

/// <summary>
/// Unknown function.
/// </summary>
[PacketHeader("f_stash_end", PacketSource.Server)]
[GenerateSerializer(true)]
public record FStashEndPacket() : IPacket;