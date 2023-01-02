//
//  FcPacketConverterTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Server.Act4;
using NosSmooth.PacketSerializer;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Extensions;
using NosSmooth.PacketSerializer.Packets;
using Xunit;

namespace NosSmooth.Packets.Tests.Converters.Packets;

/// <summary>
/// Tests FcPacketConverter.
/// </summary>
public class FcPacketConverterTests
{
    private readonly IPacketSerializer _packetSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="FcPacketConverterTests"/> class.
    /// </summary>
    public FcPacketConverterTests()
    {
        var provider = new ServiceCollection()
            .AddPacketSerialization()
            .BuildServiceProvider();

        _packetSerializer = provider.GetRequiredService<IPacketSerializer>();
        provider.GetRequiredService<IPacketTypesRepository>().AddDefaultPackets();
    }

    /// <summary>
    /// Tests that the serialization runs correctly.
    /// </summary>
    [Fact]
    public void Converter_Serialization_SerializesCorrectly()
    {
        var packet = new FcPacket(
            FactionType.Angel,
            15122,
            new FcSubPacket(76, Act4Mode.None, 0, 0, false, false, false, false, 0),
            new FcSubPacket(100, Act4Mode.Raid, 10, 1000, true, false, false, false, 0)
        );
        var packetResult = _packetSerializer.Serialize(packet);
        Assert.True(packetResult.IsSuccess);

        Assert.Equal("fc 1 15122 76 0 0 0 0 0 0 0 0 100 3 10 1000 1 0 0 0 0", packetResult.Entity);
    }

    /// <summary>
    /// Tests that the deserialization runs correctly.
    /// </summary>
    [Fact]
    public void Converter_Deserialization_DeserializesCorrectly()
    {
        var packetResult = _packetSerializer.Deserialize("fc 1 15122 76 0 0 0 0 0 0 0 0 100 3 10 1000 1 0 0 0 0", PacketSource.Server);
        Assert.True(packetResult.IsSuccess);

        var expectedPacket = new FcPacket(
            FactionType.Angel,
            15122,
            new FcSubPacket(76, Act4Mode.None, 0, 0, false, false, false, false, 0),
            new FcSubPacket(100, Act4Mode.Raid, 10, 1000, true, false, false, false, 0)
        );
        Assert.Equal(expectedPacket, packetResult.Entity);
    }
}
