//
//  PreqPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client.Movement;

/// <summary>
/// Walk through portal the character is standing on.
/// </summary>
[PacketHeader("preq", PacketSource.Server)]
[GenerateSerializer(true)]
public record PreqPacket : IPacket;