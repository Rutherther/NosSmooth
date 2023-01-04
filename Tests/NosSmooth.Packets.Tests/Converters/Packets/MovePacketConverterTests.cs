//
//  MovePacketConverterTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.Packets.Server.Entities;
using NosSmooth.PacketSerializer;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Extensions;
using NosSmooth.PacketSerializer.Packets;
using Xunit;

namespace NosSmooth.Packets.Tests.Converters.Packets;

/// <summary>
/// Tests MovePacketConverter.
/// </summary>
public class MovePacketConverterTests
{
    private readonly IPacketSerializer _packetSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="MovePacketConverterTests"/> class.
    /// </summary>
    public MovePacketConverterTests()
    {
        var provider = new ServiceCollection()
            .AddPacketSerialization()
            .BuildServiceProvider();

        _packetSerializer = provider.GetRequiredService<IPacketSerializer>();
        provider.GetRequiredService<IPacketTypesRepository>().AddDefaultPackets();
    }

    /// <summary>
    /// Tests that the converter serializes mv packet correctly.
    /// </summary>
    [Fact]
    public void Converter_Serialization_SerializesCorrectly()
    {
        MovePacket packet = new MovePacket(EntityType.Monster, 122, 15, 20, 10);
        var result = _packetSerializer.Serialize(packet);
        Assert.True(result.IsSuccess);

        Assert.Equal("mv 3 122 15 20 10", result.Entity);
    }

    /// <summary>
    /// Tests that the converter serializes mv packet correctly.
    /// </summary>
    [Fact]
    public void Converter_Deserialization_DeserializesCorrectly()
    {
        var packetString = "mv 3 122 15 20 10";
        var result = _packetSerializer.Deserialize(packetString, PacketSource.Server);
        Assert.True(result.IsSuccess);

        MovePacket actualPacket = new MovePacket(EntityType.Monster, 122, 15, 20, 10);
        Assert.Equal(result.Entity, actualPacket);
    }
}
