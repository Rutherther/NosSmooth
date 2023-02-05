//
//  FakePacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Core.Tests.Fakes.Packets;

/// <summary>
/// A fake packet.
/// </summary>
/// <param name="Input">The input.</param>
[PacketHeader("fake", PacketSource.Server)]
[GenerateSerializer(true)]
public record FakePacket
(
    [PacketGreedyIndex(0)]
    string Input
) : IPacket;