//
//  CharacterTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit.Abstractions;

namespace NosSmooth.Game.Tests.Modules;

/// <summary>
/// Test for Game.Character.
/// </summary>
public class CharacterTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterTests"/> class.
    /// </summary>
    /// <param name="testOutputHelper">The test output helper.</param>
    public CharacterTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// Tests character initialization.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task Test_Initialize()
    {
        var data = PacketFileClient.CreateFor<CharacterTests>("init", _testOutputHelper);

        await data.Client.ExecuteUntilLabelAsync("AFTER_INITIALIZED");
        data.Game.Character.ShouldNotBeNull();
        data.Game.Character.PlayerLevel.ShouldNotBeNull();
        data.Game.Character.PlayerLevel.Lvl.ShouldBe((short)99);
        data.Game.Character.JobLevel.ShouldNotBeNull();
        data.Game.Character.JobLevel.Lvl.ShouldBe((short)20);
    }
}