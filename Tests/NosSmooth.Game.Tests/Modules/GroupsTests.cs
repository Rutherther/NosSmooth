//
//  GroupsTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit.Abstractions;

namespace NosSmooth.Game.Tests.Modules;

/// <summary>
/// Tests for Game.Group.
/// </summary>
public class GroupsTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupsTests"/> class.
    /// </summary>
    /// <param name="testOutputHelper">The test output helper.</param>
    public GroupsTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// Tests packets from init_morph_use.log.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Init_Join_HpChanges()
    {
        var data = PacketFileClient.CreateFor<GroupsTests>("init_join_hp_changes", _testOutputHelper);

        data.Game.Group.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("AFTER_GROUP_JOINED");

        data.Game.Group.ShouldNotBeNull();
        data.Game.Group.Members.ShouldNotBeNull();
        data.Game.Group.Members.ShouldNotBeEmpty();
        data.Game.Group.Members.Count.ShouldBe(2);
        data.Game.Group.Members.First().Name.ShouldBe("Kekr");

        await data.Client.ExecuteUntilLabelAsync("AFTER_GROUP_FULL");

        data.Game.Group.ShouldNotBeNull();
        data.Game.Group.Members.ShouldNotBeNull();
        data.Game.Group.Members.Count.ShouldBe(3);
        data.Game.Group.Members.First().Name.ShouldBe("Kekr");
        data.Game.Group.Members.Any(x => x.Name == "fluke").ShouldBeTrue();

        await data.Client.ExecuteUntilLabelAsync("AFTER_GROUP_ONE_LEFT");

        data.Game.Group.ShouldNotBeNull();
        data.Game.Group.Members.ShouldNotBeNull();
        data.Game.Group.Members.ShouldNotBeEmpty();
        data.Game.Group.Members.Count.ShouldBe(2);
        data.Game.Group.Members.Last().Name.ShouldBe("fluke");

        await data.Client.ExecuteUntilLabelAsync("AFTER_HEALTH_LOWER");

        data.Game.Group.ShouldNotBeNull();
        data.Game.Group.Members.ShouldNotBeNull();
        data.Game.Group.Members.ShouldNotBeEmpty();
        data.Game.Group.Members.Where(x => x.Name != "dfrfgh").ShouldAllBe(x => x.Hp != null && x.Hp.Percentage < 100);

        await data.Client.ExecuteUntilLabelAsync("AFTER_HEALTH_BELOW_50");

        data.Game.Group.ShouldNotBeNull();
        data.Game.Group.Members.ShouldNotBeNull();
        data.Game.Group.Members.ShouldNotBeEmpty();
        data.Game.Group.Members.Where(x => x.Name != "dfrfgh").ShouldAllBe(x => x.Hp != null && x.Hp.Percentage < 50);
    }
}