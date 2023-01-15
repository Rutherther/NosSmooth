//
//  PtctlPacketConverterTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Packets.Client.Mates;
using NosSmooth.PacketSerializer;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Extensions;
using NosSmooth.PacketSerializer.Packets;
using Shouldly;
using Xunit;

namespace NosSmooth.Packets.Tests.Converters.Packets;

/// <summary>
/// Tests PtctlPacketConverter.
/// </summary>
public class PtctlPacketConverterTests
{
    private readonly IPacketSerializer _packetSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="PtctlPacketConverterTests"/> class.
    /// </summary>
    public PtctlPacketConverterTests()
    {
        var provider = new ServiceCollection()
            .AddPacketSerialization()
            .BuildServiceProvider();

        _packetSerializer = provider.GetRequiredService<IPacketSerializer>();
        provider.GetRequiredService<IPacketTypesRepository>().AddDefaultPackets();
    }

    /// <summary>
    /// Tests that deserialization of raid packet of list members.
    /// </summary>
    [Fact]
    public void Converter_Deserialization_Succeeds()
    {
        var packetResult = _packetSerializer.Deserialize
        (
            "ptctl 1 1 123 26 21 123 13",
            PacketSource.Server
        );
        packetResult.IsSuccess.ShouldBeTrue();
        var packet = (PtctlPacket)packetResult.Entity;
        packet.ShouldBeEquivalentTo
        (
            new PtctlPacket
            (
                1,
                1,
                new List<PtctlSubPacket>
                (
                    new[]
                    {
                        new PtctlSubPacket(123, 26, 21)
                    }
                )
            )
        );
    }
}