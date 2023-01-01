//
//  PinitPacketConverterTest.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Server.Groups;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using Xunit;

namespace NosSmooth.Packets.Tests.Converters.Packets;

/// <summary>
/// Tests PinitPacketConverter.
/// </summary>
public class PinitPacketConverterTest
{
    private readonly IPacketSerializer _packetSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="PinitPacketConverterTest"/> class.
    /// </summary>
    public PinitPacketConverterTest()
    {
        var provider = new ServiceCollection()
            .AddPacketSerialization()
            .BuildServiceProvider();

        _packetSerializer = provider.GetRequiredService<IPacketSerializer>();
    }

    /// <summary>
    /// Tests that the converter serializes mv packet correctly.
    /// </summary>
    [Fact]
    public void Converter_Serialization_SerializesCorrectly()
    {
        var packet = new PinitPacket(2, new[]
        {
            new PinitSubPacket(EntityType.Npc, 345377, 0, 83, "Kliff", -1, 319, 1, 0, null, null, null),
            new PinitSubPacket(EntityType.Npc, 345384, 1, 83, "@", -1, 2105, 0, 0, null, null, null)
        });
        var result = _packetSerializer.Serialize(packet);
        Assert.True(result.IsSuccess);

        Assert.Equal("pinit 2 2|345377|0|83|Kliff|-1|319|1|0 2|345384|1|83|@|-1|2105|0|0", result.Entity);
    }

    /// <summary>
    /// Tests that the converter serializes mv packet correctly.
    /// </summary>
    [Fact]
    public void Converter_Deserialization_DeserializesCorrectly()
    {
        var packetString = "pinit 2 2|345377|0|83|Kliff|-1|319|1|0 2|345384|1|83|@|-1|2105|0|0";
        var result = _packetSerializer.Deserialize(packetString, PacketSource.Server);
        Assert.True(result.IsSuccess);

        var actualPacket = (PinitPacket)result.Entity;

        Assert.Equal(2, actualPacket.GroupSize);
        Assert.NotNull(actualPacket.PinitSubPackets);
        Assert.Equal(2, actualPacket.PinitSubPackets!.Count);
        Assert.StrictEqual(new PinitSubPacket(EntityType.Npc, 345377, 0, 83, "Kliff", null, 319, 1, 0, null, null, null), actualPacket.PinitSubPackets[0]);
        Assert.StrictEqual(new PinitSubPacket(EntityType.Npc, 345384, 1, 83, "@", null, 2105, 0, 0, null, null, null), actualPacket.PinitSubPackets[1]);
    }
}