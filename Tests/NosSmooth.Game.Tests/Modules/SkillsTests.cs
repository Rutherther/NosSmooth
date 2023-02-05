//
//  SkillsTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit.Abstractions;

namespace NosSmooth.Game.Tests.Modules;

/// <summary>
/// Tests for Game.Skills.
/// </summary>
public class SkillsTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillsTests"/> class.
    /// </summary>
    /// <param name="testOutputHelper">The test output helper.</param>
    public SkillsTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// Tests packets from init_morph_use.log.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Init_Morph_Use()
    {
        var data = PacketFileClient.CreateFor<SkillsTests>("init_morph_use", _testOutputHelper);

        data.Game.Skills.ShouldBeNull();

        await data.Client.ExecuteUntilLabelAsync("AFTER_SKILLS_INITIALIZED");

        data.Game.Skills.ShouldNotBeNull();
        data.Game.Skills.PrimarySkill.SkillVNum.ShouldBe(240);
        data.Game.Skills.SecondarySkill.SkillVNum.ShouldBe(241);
        data.Game.Skills.OtherSkills.ShouldNotBeEmpty();
        data.Game.Skills.OtherSkills.Count.ShouldBe(1);
        data.Game.Skills.OtherSkills.Last().SkillVNum.ShouldBe(236);

        await data.Client.ExecuteUntilLabelAsync("AFTER_MORPH");

        data.Game.Skills.ShouldNotBeNull();
        data.Game.Skills.PrimarySkill.SkillVNum.ShouldBe(922);
        data.Game.Skills.SecondarySkill.SkillVNum.ShouldBe(922);
        data.Game.Skills.OtherSkills.Count.ShouldBe(10);
        data.Game.Skills.OtherSkills.ShouldAllBe(x => x.SkillVNum > 921 && x.SkillVNum < 933);

        await data.Client.ExecuteUntilLabelAsync("AFTER_MORPH_OFF");

        data.Game.Skills.ShouldNotBeNull();
        data.Game.Skills.PrimarySkill.SkillVNum.ShouldBe(240);
        data.Game.Skills.SecondarySkill.SkillVNum.ShouldBe(241);
        data.Game.Skills.OtherSkills.ShouldNotBeEmpty();
        data.Game.Skills.OtherSkills.Count.ShouldBe(1);
        data.Game.Skills.OtherSkills.Last().SkillVNum.ShouldBe(236);

        await data.Client.ExecuteUntilLabelAsync("AFTER_SKILL_USED");

        data.Game.Skills.ShouldNotBeNull();
        data.Game.Skills.PrimarySkill.IsOnCooldown.ShouldBeTrue();

        await data.Client.ExecuteUntilLabelAsync("AFTER_SKILL_REFRESHED");

        data.Game.Skills.ShouldNotBeNull();
        data.Game.Skills.PrimarySkill.IsOnCooldown.ShouldBeFalse();
    }
}