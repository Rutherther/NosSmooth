//
//  InPacketSerializerTest.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NosCore.Packets.ServerPackets.Inventory;
using NosCore.Packets.ServerPackets.Visibility;
using NosCore.Shared.Enumerations;
using NosSmooth.Core.Packets;
using NosSmooth.Core.Packets.Converters;
using Xunit;

namespace NosSmooth.Core.Tests.Packets;

/// <summary>
/// Test class for <see cref="InPacketSerializerTest"/>.
/// </summary>
public class InPacketSerializerTest
{
    private readonly InPacketSerializer _inPacketSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="InPacketSerializerTest"/> class.
    /// </summary>
    public InPacketSerializerTest()
    {
        var types = new List<Type>(new[]
        {
            typeof(InPacket),
            typeof(InAliveSubPacket),
            typeof(InNonPlayerSubPacket),
            typeof(InCharacterSubPacket),
            typeof(InItemSubPacket),
            typeof(InEquipmentSubPacket),
            typeof(UpgradeRareSubPacket),
            typeof(FamilySubPacket),
        });

        _inPacketSerializer = new ServiceCollection()
            .AddSingleton(
                p => new PacketSerializerProvider(types, types, p)
            )
            .AddSingleton<InPacketSerializer>()
            .BuildServiceProvider()
            .GetRequiredService<InPacketSerializer>();
    }

    /// <summary>
    /// Tests whether the serializer accepts to handle in packet.
    /// </summary>
    [Fact]
    public void AcceptsInPacket()
    {
        var shouldHandle = _inPacketSerializer.ShouldHandle(
            "in 1 dfrfgh - 1 79 2 6 2 1 0 106 2 -1.4480.4452.4468.4840.4132.-1.-1.-1.-1 100 100 0 -1 4 4 0 43 0 0 108 108 -1 - 26 0 0 0 0 99 0 0|0|0 0 0 10 80 0");
        Assert.True(shouldHandle);
    }

    /// <summary>
    /// Tests whether the serializer doesnt accept to handle non in packet.
    /// </summary>
    [Fact]
    public void DoesntAcceptNonInPacket()
    {
        var shouldHandle = _inPacketSerializer.ShouldHandle(
            "sr 5");
        Assert.False(shouldHandle);
    }

    /// <summary>
    /// Tests whether the result is successful when serializing player in packet.
    /// </summary>
    [Fact]
    public void SucceedsDeserializingPlayerIn()
    {
        var result = _inPacketSerializer.Deserialize(
            "in 1 dfrfgh - 1 79 2 6 2 1 0 106 2 -1.4480.4452.4468.4840.4132.-1.-1.-1.-1 50 98 0 -1 4 4 0 43 0 0 108 108 -1 - 26 0 0 0 0 99 0 0|0|0 0 0 10 80 0"
        );
        Assert.True(result.IsSuccess);
    }

    /// <summary>
    /// Tests whether the result is successful when serializing monster in packet.
    /// </summary>
    [Fact]
    public void SucceedsDeserializingMonsterIn()
    {
        var result = _inPacketSerializer.Deserialize(
            "in 2 334 1992 134 112 2 100 100 0 0 0 -1 1 0 -1 - 0 -1 0 0 0 0 0 0 0 0 0 0"
        );
        Assert.True(result.IsSuccess);
    }

    /// <summary>
    /// Tests whether the result of deserializing player is correct.
    /// </summary>
    [Fact]
    public void DeserializesPlayerInCorrectly()
    {
        var result = _inPacketSerializer.Deserialize(
            "in 1 dfrfgh - 55 79 2 6 2 1 0 106 2 -1.4480.4452.4468.4840.4132.-1.-1.-1.-1 50 95 0 -1 4 4 0 43 0 0 108 108 -1 - 26 0 0 0 0 99 0 0|0|0 0 0 10 80 0"
        ); // 55 is id, 50 hp, 95 mp

        Assert.True(result.IsSuccess);
        var inPacket = result.Entity;

        Assert.Equal(VisualType.Player, inPacket.VisualType);
        Assert.NotNull(inPacket.Name);
        Assert.Matches("dfrfgh", inPacket.Name);
        Assert.Equal(55, inPacket.VisualId);
        Assert.Equal(79, inPacket.PositionX);
        Assert.Equal(2, inPacket.PositionY);
        Assert.NotNull(inPacket.Direction);
        Assert.Equal(6, (byte)inPacket.Direction!);
        Assert.NotNull(inPacket.InCharacterSubPacket);
        var characterSubPacket = inPacket.InCharacterSubPacket!;
        Assert.Equal(AuthorityType.GameMaster, characterSubPacket.Authority);
        Assert.Equal(CharacterClassType.Archer, characterSubPacket.Class);
        Assert.NotNull(characterSubPacket.InAliveSubPacket);
        Assert.Equal(50, characterSubPacket.InAliveSubPacket!.Hp);
        Assert.Equal(95, characterSubPacket.InAliveSubPacket!.Mp);

        // TODO: check other things
    }

    /// <summary>
    /// Tests whether the result of deserializing monster is correct.
    /// </summary>
    [Fact]
    public void DeserializesMonsterInCorrectly()
    {
        var result = _inPacketSerializer.Deserialize(
            "in 2 334 1992 134 112 2 100 80 0 0 0 -1 1 0 -1 - 0 -1 0 0 0 0 0 0 0 0 0 0"
        );
        Assert.True(result.IsSuccess);
        var inPacket = result.Entity;
        Assert.Equal(VisualType.Npc, inPacket.VisualType);
        Assert.NotNull(inPacket.VNum);
        Assert.Equal(334, inPacket.VNum!.Value);
        Assert.Equal(1992, inPacket.VisualId);
        Assert.Equal(134, inPacket.PositionX);
        Assert.Equal(112, inPacket.PositionY);
        Assert.NotNull(inPacket.Direction);
        Assert.Equal(2, (byte)inPacket.Direction!);
        Assert.NotNull(inPacket.InNonPlayerSubPacket);
        var nonPlayerSubPacket = inPacket.InNonPlayerSubPacket!;
        Assert.NotNull(nonPlayerSubPacket.InAliveSubPacket);
        Assert.Equal(100, nonPlayerSubPacket.InAliveSubPacket!.Hp);
        Assert.Equal(80, nonPlayerSubPacket.InAliveSubPacket!.Mp);

        // TODO: check other things
    }
}