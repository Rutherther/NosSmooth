//
//  RaidPacketConverterTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Packets.Enums.Raids;
using NosSmooth.Packets.Server.Raids;
using NosSmooth.PacketSerializer;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Extensions;
using NosSmooth.PacketSerializer.Packets;
using Xunit;

namespace NosSmooth.Packets.Tests.Converters.Packets;

/// <summary>
/// Tests RaidPacketConverter.
/// </summary>
public class RaidPacketConverterTests
{
    private readonly IPacketSerializer _packetSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="RaidPacketConverterTests"/> class.
    /// </summary>
    public RaidPacketConverterTests()
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
    public void Converter_Deserialization_ListMembers()
    {
        var packetResult = _packetSerializer.Deserialize
        (
            "raid 0 1 2 3 4 5",
            PacketSource.Server
        );
        Assert.True(packetResult.IsSuccess);
        var packet = (RaidPacket)packetResult.Entity;
        Assert.Equal(RaidPacketType.ListMembers, packet.Type);
        Assert.NotNull(packet.ListMembersPlayerIds);
        Assert.Null(packet.LeaveType);
        Assert.Null(packet.LeaderId);
        Assert.Null(packet.PlayerHealths);

        Assert.Equal(new long[] { 1, 2, 3, 4, 5 }, packet.ListMembersPlayerIds);
    }

    /// <summary>
    /// Tests that deserialization of raid packet of leave.
    /// </summary>
    [Fact]
    public void Converter_Deserialization_Leave()
    {
        var packetResult = _packetSerializer.Deserialize
        (
            "raid 1 0",
            PacketSource.Server
        );
        Assert.True(packetResult.IsSuccess);
        var packet = (RaidPacket)packetResult.Entity;
        Assert.Equal(RaidPacketType.Leave, packet.Type);
        Assert.Null(packet.ListMembersPlayerIds);
        Assert.NotNull(packet.LeaveType);
        Assert.Null(packet.LeaderId);
        Assert.Null(packet.PlayerHealths);

        Assert.Equal(RaidLeaveType.PlayerLeft, packet.LeaveType);
    }

    /// <summary>
    /// Tests that deserialization of raid packet of leader.
    /// </summary>
    [Fact]
    public void Converter_Deserialization_Leader()
    {
        var packetResult = _packetSerializer.Deserialize
        (
            "raid 2 50",
            PacketSource.Server
        );
        Assert.True(packetResult.IsSuccess);
        var packet = (RaidPacket)packetResult.Entity;
        Assert.Equal(RaidPacketType.Leader, packet.Type);
        Assert.Null(packet.ListMembersPlayerIds);
        Assert.Null(packet.LeaveType);
        Assert.NotNull(packet.LeaderId);
        Assert.Null(packet.PlayerHealths);

        Assert.Equal(50, packet.LeaderId);
    }

    /// <summary>
    /// Tests that deserialization of raid packet of member healths.
    /// </summary>
    [Fact]
    public void Converter_Deserialization_MemberHealths()
    {
        var packetResult = _packetSerializer.Deserialize
        (
            "raid 3 1.100.100 2.90.95 3.95.90",
            PacketSource.Server
        );
        Assert.True(packetResult.IsSuccess);
        var packet = (RaidPacket)packetResult.Entity;
        Assert.Equal(RaidPacketType.PlayerHealths, packet.Type);
        Assert.Null(packet.ListMembersPlayerIds);
        Assert.Null(packet.LeaveType);
        Assert.Null(packet.LeaderId);
        Assert.NotNull(packet.PlayerHealths);

        Assert.Equal
        (
            new[]
            {
                new RaidPlayerHealthsSubPacket(1, 100, 100),
                new RaidPlayerHealthsSubPacket(2, 90, 95),
                new RaidPlayerHealthsSubPacket(3, 95, 90)
            },
            packet.PlayerHealths
        );
    }
}