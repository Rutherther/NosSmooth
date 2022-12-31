//
//  SkiPacketConverterTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Extensions;
using NosSmooth.Packets.Packets;
using NosSmooth.Packets.Server.Groups;
using NosSmooth.Packets.Server.Skills;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using Xunit;

namespace NosSmooth.Packets.Tests.Converters.Packets;

/// <summary>
/// Tests ski packet serializer.
/// </summary>
public class SkiPacketConverterTests
{
    private readonly IPacketSerializer _packetSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkiPacketConverterTests"/> class.
    /// </summary>
    public SkiPacketConverterTests()
    {
        var provider = new ServiceCollection()
            .AddPacketSerialization()
            .BuildServiceProvider();

        _packetSerializer = provider.GetRequiredService<IPacketSerializer>();
        provider.GetRequiredService<IPacketTypesRepository>().AddDefaultPackets();
    }

    /// <summary>
    /// Tests that ski packet is deserialized correctly.
    /// </summary>
    [Fact]
    public void Converter_Deserialization_DeserializesCorrectly()
    {
        var deserialized = _packetSerializer.Deserialize
        (
            "ski 0 220 221 220 221 697|4 706|0 310 311",
            PacketSource.Server
        );

        Assert.True(deserialized.IsSuccess);
        var expected = new SkiPacket
        (
            0,
            220,
            221,
            new[]
            {
                new SkiSubPacket(220, null), new SkiSubPacket(221, null), new SkiSubPacket(697, 4),
                new SkiSubPacket(706, 0), new SkiSubPacket(310, null), new SkiSubPacket(311, null)
            }
        );
        var skiPacket = (SkiPacket)deserialized.Entity;
        Assert.Equal(expected.PrimarySkillVNum, skiPacket.PrimarySkillVNum);
        Assert.Equal(expected.SecondarySkillVNum, skiPacket.SecondarySkillVNum);
        Assert.Equal(expected.SkillSubPackets, skiPacket.SkillSubPackets);
    }
}