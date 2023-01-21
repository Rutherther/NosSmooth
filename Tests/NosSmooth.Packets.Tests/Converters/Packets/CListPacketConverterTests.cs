//
//  CListPacketConverterTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Packets.Enums.Players;
using NosSmooth.Packets.Server.Login;
using NosSmooth.PacketSerializer;
using NosSmooth.PacketSerializer.Abstractions;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Extensions;
using NosSmooth.PacketSerializer.Packets;
using Shouldly;
using Xunit;

namespace NosSmooth.Packets.Tests.Converters.Packets;

/// <summary>
/// Tests CListPacketConverter.
/// </summary>
public class CListPacketConverterTests
{
    private readonly IPacketSerializer _packetSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="CListPacketConverterTests"/> class.
    /// </summary>
    public CListPacketConverterTests()
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
        var packet = new CListPacket
        (
            1,
            "derfy",
            0,
            SexType.Female,
            HairStyle.HairStyleA,
            HairColor.FlashPurple,
            0,
            PlayerClass.Archer,
            99,
            80,
            new CListEquipmentSubPacket
            (
                null,
                null,
                4452,
                4468,
                4840,
                4131,
                null,
                null
            ),
            99,
            1,
            1,
            new OptionalWrapper<NullableWrapper<CListPetSubPacket>>[]
            {
                new NullableWrapper<CListPetSubPacket>(new CListPetSubPacket(0, 2105)),
                new NullableWrapper<CListPetSubPacket>(new CListPetSubPacket(0, 319)),
                new NullableWrapper<CListPetSubPacket>(new CListPetSubPacket(0, 2106)),
                new NullableWrapper<CListPetSubPacket>(new CListPetSubPacket(0, 2107)),
                new NullableWrapper<CListPetSubPacket>(new CListPetSubPacket(0, 2108)),
                new NullableWrapper<CListPetSubPacket>(new CListPetSubPacket(0, 2100)),
                new NullableWrapper<CListPetSubPacket>(new CListPetSubPacket(0, 2102)),
                new NullableWrapper<CListPetSubPacket>(new CListPetSubPacket(0, 317)),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
            },
            0,
            0
        );
        var packetResult = _packetSerializer.Serialize(packet);
        Assert.True(packetResult.IsSuccess);

        Assert.Equal
        (
            "clist 1 derfy 0 1 0 106 0 2 99 80 -1.-1.4452.4468.4840.4131.-1.-1 99  1 1 0.2105.0.319.0.2106.0.2107.0.2108.0.2100.0.2102.0.317.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1 0 0",
            packetResult.Entity
        );
    }

    /// <summary>
    /// Tests that the deserialization runs correctly.
    /// </summary>
    [Fact]
    public void Converter_Deserialization_DeserializesCorrectly()
    {
        var packetResult = _packetSerializer.Deserialize
        (
            "clist 1 derfy 0 1 0 106 0 2 99 80 -1.-1.4452.4468.4840.4131.-1.-1 99  1 1 0.2105.0.319.0.2106.0.2107.0.2108.0.2100.0.2102.0.317.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1 0 0",
            PacketSource.Server
        );
        packetResult.IsSuccess.ShouldBeTrue();

        var expectedPacket = new CListPacket
        (
            1,
            "derfy",
            0,
            SexType.Female,
            HairStyle.HairStyleA,
            HairColor.FlashPurple,
            0,
            PlayerClass.Archer,
            99,
            80,
            new CListEquipmentSubPacket
            (
                null,
                null,
                4452,
                4468,
                4840,
                4131,
                null,
                null
            ),
            99,
            1,
            1,
            new List<OptionalWrapper<NullableWrapper<CListPetSubPacket>>>(new OptionalWrapper<NullableWrapper<CListPetSubPacket>>[]
            {
                new NullableWrapper<CListPetSubPacket>(new CListPetSubPacket(0, 2105)),
                new NullableWrapper<CListPetSubPacket>(new CListPetSubPacket(0, 319)),
                new NullableWrapper<CListPetSubPacket>(new CListPetSubPacket(0, 2106)),
                new NullableWrapper<CListPetSubPacket>(new CListPetSubPacket(0, 2107)),
                new NullableWrapper<CListPetSubPacket>(new CListPetSubPacket(0, 2108)),
                new NullableWrapper<CListPetSubPacket>(new CListPetSubPacket(0, 2100)),
                new NullableWrapper<CListPetSubPacket>(new CListPetSubPacket(0, 2102)),
                new NullableWrapper<CListPetSubPacket>(new CListPetSubPacket(0, 317)),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
                new NullableWrapper<CListPetSubPacket>(null),
            }),
            0,
            0
        );
        packetResult.Entity.ShouldBeEquivalentTo(expectedPacket);
    }

    /// <summary>
    /// Tests that deserialization of pets list with optional values works correctly.
    /// </summary>
    [Fact]
    public void Converter_Deserialization_PresentFalseForNotPresent()
    {
        var packetResult = _packetSerializer.Deserialize
        (
            "clist 2 KexpExp 0 1 0 9 0 0 22 0 -1.12.1.8.-1.-1.-1.-1.-1.-1 20  1 1 -1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1. 0 0",
            PacketSource.Server
        );
        Assert.True(packetResult.IsSuccess);
        var packet = (CListPacket)packetResult.Entity;
        Assert.Equal(27, packet.PetsSubPacket.Count);

        Assert.False(packet.PetsSubPacket.Last().Present);
        for (var i = 0; i < 26; i++)
        {
            Assert.True(packet.PetsSubPacket[i].Present);
            Assert.True(packet.PetsSubPacket[i].Value.Value is null);
        }
    }
}