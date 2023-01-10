//
//  FriendsTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit.Abstractions;
using Xunit.Sdk;

namespace NosSmooth.Game.Tests.Modules;

/// <summary>
/// Test for Game.Friends.
/// </summary>
public class FriendsTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="FriendsTests"/> class.
    /// </summary>
    /// <param name="testOutputHelper">The test output helper.</param>
    public FriendsTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// Tests friends initialization, friend logout, friend login.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task Test_Init_On_Off()
    {
        var data = PacketFileClient.CreateFor<FriendsTests>("init_off_on", _testOutputHelper);

        data.Game.Friends.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("AFTER_FRIENDS_INITIALIZED");

        data.Game.Friends.ShouldNotBeNull();
        data.Game.Friends.ShouldNotBeEmpty();
        data.Game.Friends.Count.ShouldBe(3);
        data.Game.Friends.ShouldContain(p => p.CharacterName == "ffff");
        data.Game.Friends.ShouldContain(p => p.CharacterName == "fluke");
        data.Game.Friends.ShouldContain(p => p.CharacterName == "like");

        data.Game.Friends.ShouldAllBe(x => x.CharacterName == "fluke" || !x.IsConnected);
        data.Game.Friends.First(x => x.CharacterName == "fluke").IsConnected.ShouldBeTrue();

        await data.Client.ExecuteUntilLabelAsync("AFTER_FRIENDS_OFFLINE");

        data.Game.Friends.ShouldNotBeNull();
        data.Game.Friends.ShouldNotBeEmpty();
        data.Game.Friends.Count.ShouldBe(3);
        data.Game.Friends.ShouldAllBe(x => !x.IsConnected);

        await data.Client.ExecuteUntilLabelAsync("AFTER_FRIEND_LIKE_ONLINE");

        data.Game.Friends.ShouldNotBeNull();
        data.Game.Friends.ShouldNotBeEmpty();
        data.Game.Friends.Count.ShouldBe(3);
        data.Game.Friends.First(x => x.CharacterName == "like").IsConnected.ShouldBeTrue();
    }
}