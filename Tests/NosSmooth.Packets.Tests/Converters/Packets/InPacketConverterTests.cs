//
//  InPacketConverterTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Packets.Attributes;
using NosSmooth.Packets.Converters;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.Packets.Enums.Players;
using NosSmooth.Packets.Extensions;
using NosSmooth.Packets.Packets.Server.Entities;
using NosSmooth.Packets.Packets.Server.Entities.Generated;
using NosSmooth.Packets.Packets.Server.Players;
using NosSmooth.Packets.Packets.Server.Weapons;
using Xunit;

namespace NosSmooth.Packets.Tests.Converters.Packets;

/// <summary>
/// Tests <see cref="InPacketConverter"/>.
/// </summary>
public class InPacketConverterTests
{
    private readonly IPacketSerializer _packetSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="InPacketConverterTests"/> class.
    /// </summary>
    public InPacketConverterTests()
    {
        var provider = new ServiceCollection()
            .AddPacketSerialization()
            .BuildServiceProvider();

        _packetSerializer = provider.GetRequiredService<IPacketSerializer>();
    }

    /// <summary>
    /// Checks that the converter deserializes in player packet correctly.
    /// </summary>
    [Fact]
    public void DeserializesPlayerInCorrectly()
    {
        var packetString
            = "in 1 dfrfgh - 55 79 2 6 2 1 0 106 2 -1.4480.4452.4468.4840.4132.-1.-1.-1.-1 50 95 0 -1 4 4 0 43 0 0 108 108 -1 - 26 0 0 0 0 99 0 0|0|0 0 0 10 80 0";
        var result = _packetSerializer.Deserialize(packetString, PacketSource.Server);
        var expectedPacket = new InPacket
        (
            EntityType.Player,
            "dfrfgh",
            null,
            null,
            55,
            79,
            2,
            6,
            new InPlayerSubPacket
            (
                AuthorityType.GameMaster,
                SexType.Female,
                HairStyle.HairStyleA,
                HairColor.FlashPurple,
                PlayerClass.Archer,
                new InEquipmentSubPacket
                (
                    -1,
                    4480,
                    4452,
                    4468,
                    4840,
                    4132,
                    -1,
                    -1,
                    -1,
                    -1
                ),
                50,
                95,
                false,
                -1,
                4,
                Element.Dark,
                0,
                43,
                0,
                0,
                new UpgradeRareSubPacket(10, 8),
                new UpgradeRareSubPacket(10, 8),
                new FamilySubPacket("-1", null),
                null,
                "26",
                false,
                0,
                0,
                0,
                99,
                0,
                new List<bool>(new[] { false, false, false }),
                false,
                0,
                10,
                80,
                0
            ),
            null,
            null
        );
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedPacket, result.Entity);
    }

    /// <summary>
    /// Checks that the converter serializes in player packet correctly.
    /// </summary>
    [Fact]
    public void SerializesPlayerInCorrectly()
    {
        var packet = new InPacket
        (
            EntityType.Player,
            "dfrfgh",
            null,
            null,
            55,
            79,
            2,
            6,
            new InPlayerSubPacket
            (
                AuthorityType.GameMaster,
                SexType.Female,
                HairStyle.HairStyleA,
                HairColor.FlashPurple,
                PlayerClass.Archer,
                new InEquipmentSubPacket
                (
                    -1,
                    4480,
                    4452,
                    4468,
                    4840,
                    4132,
                    -1,
                    -1,
                    -1,
                    -1
                ),
                50,
                95,
                false,
                -1,
                4,
                Element.Dark,
                0,
                43,
                0,
                0,
                new UpgradeRareSubPacket(10, 8),
                new UpgradeRareSubPacket(10, 8),
                new FamilySubPacket("-1", null),
                null,
                "26",
                false,
                0,
                0,
                0,
                99,
                0,
                new List<bool>(new[] { false, false, false }),
                false,
                0,
                10,
                80,
                0
            ),
            null,
            null
        );
        var result = _packetSerializer.Serialize(packet);
        var expectedPacketString
            = "in 1 dfrfgh - 55 79 2 6 2 1 0 106 2 -1.4480.4452.4468.4840.4132.-1.-1.-1.-1 50 95 0 -1 4 4 0 43 0 0 108 108 -1 - 26 0 0 0 0 99 0 0|0|0 0 0 10 80 0";
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedPacketString, result.Entity);
    }

    /// <summary>
    /// Checks that the converter deserializes in npc packet correctly.
    /// </summary>
    [Fact]
    public void DeserializesNpcInCorrectly()
    {
        var packetString
            = "in 2 334 1992 134 112 2 100 80 0 0 0 -1 1 0 -1 - 0 -1 0 0 0 0 0 0 0 0 0 0";
        var result = _packetSerializer.Deserialize(packetString, PacketSource.Server);
        var expectedPacket = new InPacket
        (
            EntityType.Npc,
            null,
            334,
            null,
            1992,
            134,
            112,
            2,
            null,
            null,
            new InNonPlayerSubPacket
            (
                100,
                80,
                0,
                0,
                0,
                -1,
                SpawnEffect.NoEffect,
                false,
                -1,
                null,
                "0",
                "-1",
                "0",
                0,
                0,
                0,
                0,
                0,
                0,
                false,
                "0",
                "0"
            )
        );

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedPacket, result.Entity);
    }

    /// <summary>
    /// Checks that the converter serializes in npc packet correctly.
    /// </summary>
    [Fact]
    public void SerializesNpcInCorrectly()
    {
        var actualPacket = new InPacket
        (
            EntityType.Npc,
            null,
            334,
            null,
            1992,
            134,
            112,
            2,
            null,
            null,
            new InNonPlayerSubPacket
            (
                100,
                80,
                0,
                0,
                0,
                -1,
                SpawnEffect.NoEffect,
                false,
                -1,
                null,
                "0",
                "-1",
                "0",
                0,
                0,
                0,
                0,
                0,
                0,
                false,
                "0",
                "0"
            )
        );

        var result = _packetSerializer.Serialize(actualPacket);
        Assert.True(result.IsSuccess);
        var expectedPacketString
            = "in 2 334 1992 134 112 2 100 80 0 0 0 -1 1 0 -1 - 0 -1 0 0 0 0 0 0 0 0 0 0";
        Assert.Equal(expectedPacketString, result.Entity);
    }
}