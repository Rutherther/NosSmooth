//
//  InPacketConverterTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.Packets.Enums.Players;
using NosSmooth.Packets.Server.Character;
using NosSmooth.Packets.Server.Maps;
using NosSmooth.Packets.Server.Weapons;
using NosSmooth.PacketSerializer;
using NosSmooth.PacketSerializer.Abstractions;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Extensions;
using NosSmooth.PacketSerializer.Packets;
using Shouldly;
using Xunit;

namespace NosSmooth.Packets.Tests.Converters.Packets;

/// <summary>
/// Tests InPacketConverter.
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
        provider.GetRequiredService<IPacketTypesRepository>().AddDefaultPackets();
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
                    null,
                    4480,
                    4452,
                    4468,
                    4840,
                    4132,
                    null,
                    null,
                    null,
                    null
                ),
                50,
                95,
                false,
                null,
                4,
                Element.Dark,
                0,
                43,
                0,
                0,
                new UpgradeRareSubPacket(10, 8),
                new UpgradeRareSubPacket(10, 8),
                new NullableWrapper<FamilySubPacket>(null),
                null,
                26,
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
        result.IsSuccess.ShouldBeTrue();
        var packet = (InPacket)result.Entity;
        packet.EntityType.ShouldBe(expectedPacket.EntityType);
        packet.EntityId.ShouldBe(expectedPacket.EntityId);
        packet.ItemSubPacket.ShouldBe(expectedPacket.ItemSubPacket);
        packet.Direction.ShouldBe(expectedPacket.Direction);
        packet.Name.ShouldBe(expectedPacket.Name);
        packet.PositionX.ShouldBe(expectedPacket.PositionX);
        packet.PositionY.ShouldBe(expectedPacket.PositionY);
        packet.VNum.ShouldBe(expectedPacket.VNum);
        packet.NonPlayerSubPacket.ShouldBe(expectedPacket.NonPlayerSubPacket);

        packet.PlayerSubPacket.ShouldNotBeNull();
        var playerSubPacket = packet.PlayerSubPacket!;
        var expectedSubPacket = packet.PlayerSubPacket!;

        playerSubPacket.Level.ShouldBe(expectedSubPacket.Level);
        playerSubPacket.FamilySubPacket.ShouldBe(expectedSubPacket.FamilySubPacket);
        playerSubPacket.HeroLevel.ShouldBe(expectedSubPacket.HeroLevel);
        playerSubPacket.Size.ShouldBe(expectedSubPacket.Size);
        playerSubPacket.FamilyLevel.ShouldBe(expectedSubPacket.FamilyLevel);
        playerSubPacket.MorphUpgrade.ShouldBe(expectedSubPacket.MorphUpgrade);
        playerSubPacket.MorphUpgrade2.ShouldBe(expectedSubPacket.MorphUpgrade2);
        playerSubPacket.Authority.ShouldBe(expectedSubPacket.Authority);
        playerSubPacket.Class.ShouldBe(expectedSubPacket.Class);
        playerSubPacket.Compliment.ShouldBe(expectedSubPacket.Compliment);
        playerSubPacket.Equipment.ShouldBe(expectedSubPacket.Equipment);
        playerSubPacket.Faction.ShouldBe(expectedSubPacket.Faction);
        playerSubPacket.Fairy.ShouldBe(expectedSubPacket.Fairy);
        playerSubPacket.Sex.ShouldBe(expectedSubPacket.Sex);
        playerSubPacket.ArenaWinner.ShouldBe(expectedSubPacket.ArenaWinner);
        playerSubPacket.FairyElement.ShouldBe(expectedSubPacket.FairyElement);
        playerSubPacket.FamilyIcons.ShouldBe(expectedSubPacket.FamilyIcons);
        playerSubPacket.FamilyName.ShouldBe(expectedSubPacket.FamilyName);
        playerSubPacket.GroupId.ShouldBe(expectedSubPacket.GroupId);
        playerSubPacket.HairColor.ShouldBe(expectedSubPacket.HairColor);
        playerSubPacket.HairStyle.ShouldBe(expectedSubPacket.HairStyle);
        playerSubPacket.HpPercentage.ShouldBe(expectedSubPacket.HpPercentage);
        playerSubPacket.IsInvisible.ShouldBe(expectedSubPacket.IsInvisible);
        playerSubPacket.IsSitting.ShouldBe(expectedSubPacket.IsSitting);
        playerSubPacket.MpPercentage.ShouldBe(expectedSubPacket.MpPercentage);
        playerSubPacket.ReputationIcon.ShouldBe(expectedSubPacket.ReputationIcon);
        playerSubPacket.FamilySubPacket.ShouldBe(expectedSubPacket.FamilySubPacket);
        playerSubPacket.ArmorUpgradeRareSubPacket.ShouldBe(expectedSubPacket.ArmorUpgradeRareSubPacket);
        playerSubPacket.WeaponUpgradeRareSubPacket.ShouldBe(expectedSubPacket.WeaponUpgradeRareSubPacket);
        playerSubPacket.MorphVNum.ShouldBe(expectedSubPacket.MorphVNum);
    }

    /// <summary>
    /// Checks that packet with UpgradeRareSubPacket 0 deserializes.
    /// </summary>
    [Fact]
    public void DeserializesPlayerSuccessfully()
    {
        var packetString
            = "in 1 fairy50% - 6893217 140 148 2 0 1 0 9 0 -1.12.1.8.-1.4131.-1.-1.-1.-1 100 100 0 1 4 3 0 42 1 0 0 0 -1 - 1 0 0 0 0 9 0 0|0|0 0 0 10 0 0";
        var result = _packetSerializer.Deserialize(packetString, PacketSource.Server);
        Assert.True(result.IsSuccess);
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
                new NullableWrapper<FamilySubPacket>(null),
                null,
                26,
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
                null,
                SpawnEffect.NoEffect,
                false,
                null,
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
        result.IsSuccess.ShouldBeTrue();
        result.Entity.ShouldBe(expectedPacket);
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
                null,
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
            = "in 2 334 1992 134 112 2 100 80 0 0 0 -1 1 0 -1 - 0 - 0 0 0 0 0 0 0 0 0 0";
        Assert.Equal(expectedPacketString, result.Entity);
    }
}
