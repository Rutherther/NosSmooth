//
//  OkPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Client.Login;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Login;

/// <summary>
/// The selection of character is approved.
/// </summary>
/// <remarks>
/// Sent after <see cref="SelectPacket"/>.
/// </remarks>
[PacketHeader("OK", PacketSource.Server)]
[GenerateSerializer(true)]
public record OkPacket() : IPacket;