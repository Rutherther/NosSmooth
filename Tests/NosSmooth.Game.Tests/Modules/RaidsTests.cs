//
//  RaidsTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks.Dataflow;
using NosSmooth.Game.Data.Raids;
using NosSmooth.Packets.Enums.Raids;
using Xunit.Abstractions;

namespace NosSmooth.Game.Tests.Modules;

/// <summary>
/// Tests Game.CurrentRaid.
/// </summary>
public class RaidsTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="RaidsTests"/> class.
    /// </summary>
    /// <param name="testOutputHelper">The test output helper.</param>
    public RaidsTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// Tests join_leave.plog.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Test_Join_Leave()
    {
        var data = PacketFileClient.CreateFor<RaidsTests>("join_leave", _testOutputHelper);

        data.Game.CurrentRaid.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("RAID_JOINED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.Leader.ShouldNotBeNull();
        data.Game.CurrentRaid.Members.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.Waiting);

        await data.Client.ExecuteUntilLabelAsync("RAID_BEFORE_FINISHED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.Waiting);

        await data.Client.ExecuteUntilLabelAsync("RAID_FINISHED");

        data.Game.CurrentRaid.ShouldBeNull();
    }

    /// <summary>
    /// Tests belial_success.plog.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Test_Belial_Success()
    {
        var data = PacketFileClient.CreateFor<RaidsTests>("belial_success", _testOutputHelper);

        data.Game.CurrentRaid.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("RAID_JOINED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.Leader.ShouldNotBeNull();
        data.Game.CurrentRaid.Members.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.Waiting);
        data.Game.CurrentRaid.Type.ShouldBe(RaidType.Belial);
        data.Game.CurrentRaid.Bosses.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("RAID_STARTED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.Started);

        await data.Client.ExecuteUntilLabelAsync("RAID_BOSS_FIGHT");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.Bosses.ShouldNotBeEmpty();
        data.Game.CurrentRaid.Boss.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.BossFight);

        await data.Client.ExecuteUntilLabelAsync("RAID_CLEARED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.EndedSuccessfully);
    }

    /// <summary>
    /// Tests carno_leave_early.plog.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Test_Carno_Leave_Early()
    {
        var data = PacketFileClient.CreateFor<RaidsTests>("carno_leave_early", _testOutputHelper);

        data.Game.CurrentRaid.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("RAID_JOINED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.Leader.ShouldNotBeNull();
        data.Game.CurrentRaid.Members.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.Waiting);
        data.Game.CurrentRaid.Type.ShouldBe(RaidType.Carno);
        data.Game.CurrentRaid.Bosses.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("RAID_STARTED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.Started);

        await data.Client.ExecuteUntilLabelAsync("RAID_BOSS_FIGHT");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.Bosses.ShouldNotBeEmpty();
        data.Game.CurrentRaid.Boss.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.BossFight);

        await data.Client.ExecuteUntilLabelAsync("RAID_BEFORE_FINISHED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.BossFight);

        await data.Client.ExecuteUntilLabelAsync("RAID_FINISHED");

        data.Game.CurrentRaid.ShouldBeNull();
    }

    /// <summary>
    /// Tests cuby_leave_early.plog.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Test_Cuby_Leave_Early()
    {
        var data = PacketFileClient.CreateFor<RaidsTests>("cuby_leave_early", _testOutputHelper);

        data.Game.CurrentRaid.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("RAID_JOINED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.Leader.ShouldNotBeNull();
        data.Game.CurrentRaid.Members.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.Waiting);
        data.Game.CurrentRaid.Type.ShouldBe(RaidType.Cuby);
        data.Game.CurrentRaid.Bosses.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("RAID_STARTED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.Started);

        await data.Client.ExecuteUntilLabelAsync("RAID_BEFORE_FINISHED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.Started);

        await data.Client.ExecuteUntilLabelAsync("RAID_FINISHED");

        data.Game.CurrentRaid.ShouldBeNull();
    }

    /// <summary>
    /// Tests cuby_success.plog.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Test_Cuby_Success()
    {
        var data = PacketFileClient.CreateFor<RaidsTests>("cuby_success", _testOutputHelper);

        data.Game.CurrentRaid.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("RAID_JOINED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.Members.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.Waiting);
        data.Game.CurrentRaid.Type.ShouldBe(RaidType.Cuby);
        data.Game.CurrentRaid.Bosses.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("RAID_STARTED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.Started);

        await data.Client.ExecuteUntilLabelAsync("RAID_BOSS_FIGHT");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.Bosses.ShouldNotBeEmpty();
        data.Game.CurrentRaid.Boss.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.BossFight);

        await data.Client.ExecuteUntilLabelAsync("RAID_CLEARED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.EndedSuccessfully);
    }

    /// <summary>
    /// Tests cuby_success_not_in_boss_map.plog.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Test_Cuby_Success_Not_In_Boss_Map()
    {
        var data = PacketFileClient.CreateFor<RaidsTests>("cuby_success_not_in_boss_map", _testOutputHelper);

        data.Game.CurrentRaid.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("RAID_JOINED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.Leader.ShouldNotBeNull();
        data.Game.CurrentRaid.Members.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.Waiting);
        data.Game.CurrentRaid.Type.ShouldBe(RaidType.Cuby);
        data.Game.CurrentRaid.Bosses.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("RAID_STARTED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.Started);

        await data.Client.ExecuteUntilLabelAsync("RAID_CLEARED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.EndedSuccessfully);

        await data.Client.ExecuteUntilLabelAsync("RAID_BEFORE_FINISHED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.EndedSuccessfully);

        await data.Client.ExecuteUntilLabelAsync("RAID_FINISHED");

        data.Game.CurrentRaid.ShouldBeNull();
    }

    /// <summary>
    /// Tests fafnir_fail.plog.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Test_Fafnir_Fail()
    {
        var data = PacketFileClient.CreateFor<RaidsTests>("fafnir_fail", _testOutputHelper);

        data.Game.CurrentRaid.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("RAID_JOINED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.Leader.ShouldNotBeNull();
        data.Game.CurrentRaid.Members.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.Waiting);
        data.Game.CurrentRaid.Type.ShouldBe(RaidType.Fafnir);
        data.Game.CurrentRaid.Bosses.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("RAID_STARTED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.Started);

        await data.Client.ExecuteUntilLabelAsync("RAID_FAILED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.TeamFailed);

        await data.Client.ExecuteUntilLabelAsync("RAID_BEFORE_FINISHED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.TeamFailed);

        await data.Client.ExecuteUntilLabelAsync("RAID_FINISHED");

        data.Game.CurrentRaid.ShouldBeNull();
    }

    /// <summary>
    /// Tests hongbi_leave_early.plog.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Test_Hongbi_Leave_Early()
    {
        var data = PacketFileClient.CreateFor<RaidsTests>("hongbi_leave_early", _testOutputHelper);

        data.Game.CurrentRaid.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("RAID_JOINED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.Leader.ShouldNotBeNull();
        data.Game.CurrentRaid.Members.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.Waiting);
        data.Game.CurrentRaid.Type.ShouldBe(RaidType.HongbiCheongbi);
        data.Game.CurrentRaid.Bosses.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("RAID_STARTED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.Started);

        await data.Client.ExecuteUntilLabelAsync("RAID_BOSS_FIGHT");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.Bosses.ShouldNotBeEmpty();
        data.Game.CurrentRaid.Boss.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.BossFight);

        await data.Client.ExecuteUntilLabelAsync("RAID_BEFORE_FINISHED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.BossFight);

        await data.Client.ExecuteUntilLabelAsync("RAID_FINISHED");

        data.Game.CurrentRaid.ShouldBeNull();
    }

    /// <summary>
    /// Tests hongbi_success.plog.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Test_Hongbi_Success()
    {
        var data = PacketFileClient.CreateFor<RaidsTests>("hongbi_success", _testOutputHelper);

        data.Game.CurrentRaid.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("RAID_JOINED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.Members.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.Waiting);
        data.Game.CurrentRaid.Type.ShouldBe(RaidType.HongbiCheongbi);
        data.Game.CurrentRaid.Bosses.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("RAID_STARTED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.Started);

        await data.Client.ExecuteUntilLabelAsync("RAID_BOSS_FIGHT");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.Bosses.ShouldNotBeEmpty();
        data.Game.CurrentRaid.Bosses.Count.ShouldBe(2);
        data.Game.CurrentRaid.Boss.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.BossFight);
        var boss = data.Game.CurrentRaid.Boss;

        await data.Client.ExecuteUntilLabelAsync("RAID_CLEARED");

        data.Game.CurrentRaid.Boss.ShouldBe(boss);
        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.Bosses.Count.ShouldBe(2);
        data.Game.CurrentRaid.State.ShouldBe(RaidState.EndedSuccessfully);

        await data.Client.ExecuteUntilLabelAsync("RAID_BEFORE_FINISHED");

        data.Game.CurrentRaid.ShouldNotBeNull();
        data.Game.CurrentRaid.State.ShouldBe(RaidState.EndedSuccessfully);

        await data.Client.ExecuteUntilLabelAsync("RAID_FINISHED");

        data.Game.CurrentRaid.ShouldBeNull();
    }
}