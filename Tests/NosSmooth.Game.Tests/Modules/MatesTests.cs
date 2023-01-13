//
//  MatesTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Mates;
using Shouldly;
using Xunit.Abstractions;

namespace NosSmooth.Game.Tests.Modules;

/// <summary>
/// Tests for Game.Mates.
/// </summary>
public class MatesTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="MatesTests"/> class.
    /// </summary>
    /// <param name="testOutputHelper">The test output helper.</param>
    public MatesTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// Tests packets from Inferno_No_Partner.log.
    /// Tests that Inferno is detected correctly,
    /// no partner is detected correctly. No pet skills. No partner skills.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Test_Inferno_No_Partner()
    {
        var data = PacketFileClient.CreateFor<MatesTests>("Inferno_No_Partner", _testOutputHelper);

        data.Game.Mates.ShouldBeNull();

        // Pets and partners initialized
        await data.Client.ExecuteUntilLabelAsync("AFTER_MATES_INITIALIZED");
        data.Game.Mates.ShouldNotBeNull();
        data.Game.Mates.Pets.Count().ShouldBe(1);
        data.Game.Mates.Pets.First().NpcVNum.ShouldBe(2105 /*inferno*/);

        // Having Inferno as current pet.
        await data.Client.ExecuteUntilLabelAsync("AFTER_INFERNO_COMPANY");

        data.Game.Mates.ShouldNotBeNull();
        data.Game.Mates.CurrentPartner.ShouldBeNull();
        data.Game.Mates.CurrentPet.ShouldNotBeNull();

        data.Game.Mates.CurrentPet.Pet.NpcVNum.ShouldBe(2105 /*inferno*/);

        data.Game.Mates.PartnerSkills.ShouldBeNull();
        data.Game.Mates.PetSkill.ShouldBeNull();

        await data.Client.ExecuteToEnd();

        data.Game.Mates.ShouldNotBeNull();
        data.Game.Mates.CurrentPet.ShouldBeNull();
        data.Game.Mates.CurrentPartner.ShouldBeNull();
        data.Game.Mates.PartnerSkills.ShouldBeNull();
        data.Game.Mates.PetSkill.ShouldBeNull();
    }

    /// <summary>
    /// Tests packets from Otter_and_Graham_Yuna.log.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Test_Otter_And_Graham_Yuna()
    {
        var data = PacketFileClient.CreateFor<MatesTests>("Otter_and_Graham_Yuna", _testOutputHelper);

        data.Game.Mates.ShouldBeNull();

        // Pets and partners initialized
        await data.Client.ExecuteUntilLabelAsync("AFTER_MATES_INITIALIZED");
        data.Game.Mates.ShouldNotBeNull();
        data.Game.Mates.Pets.ShouldContain(pred => pred.NpcVNum == 683 /*otter*/);
        data.Game.Mates.Partners.ShouldContain(pred => pred.NpcVNum == 2557 /*graham*/);

        var graham = data.Game.Mates.Partners.First(x => x.NpcVNum == 2557);

        graham.Sp.ShouldNotBeNull();
        graham.Sp.VNum.ShouldBe(4405);
        graham.Sp.Skill1.ShouldNotBeNull();
        graham.Sp.Skill1.SkillVNum.ShouldBe(1602);
        graham.Sp.Skill1.Rank.ShouldBe(PartnerSkillRank.B);
        graham.Sp.Skill2.ShouldNotBeNull();
        graham.Sp.Skill2.SkillVNum.ShouldBe(1603);
        graham.Sp.Skill2.Rank.ShouldBe(PartnerSkillRank.D);
        graham.Sp.Skill3.ShouldNotBeNull();
        graham.Sp.Skill3.SkillVNum.ShouldBe(1604);
        graham.Sp.Skill3.Rank.ShouldBe(PartnerSkillRank.S);

        // Having Graham and Otter as current mates
        await data.Client.ExecuteUntilLabelAsync("AFTER_MATES_COMPANY");

        data.Game.Mates.ShouldNotBeNull();
        data.Game.Mates.CurrentPartner.ShouldNotBeNull();
        data.Game.Mates.CurrentPet.ShouldNotBeNull();

        data.Game.Mates.CurrentPet.Pet.NpcVNum.ShouldBe(683 /*otter*/);
        data.Game.Mates.CurrentPartner.Partner.NpcVNum.ShouldBe(2557 /*graham*/);

        data.Game.Mates.PartnerSkills.ShouldBeNull();
        data.Game.Mates.PetSkill.ShouldNotBeNull();
        data.Game.Mates.PetSkill.SkillVNum.ShouldBe(663);

        await data.Client.ExecuteUntilLabelAsync("AFTER_YUNA_SP");

        data.Game.Mates.ShouldNotBeNull();
        data.Game.Mates.CurrentPartner.ShouldNotBeNull();
        data.Game.Mates.PartnerSkills.ShouldNotBeNull();
        data.Game.Mates.PartnerSkills.Count.ShouldBe(3);
        data.Game.Mates.PartnerSkills[0].SkillVNum.ShouldBe(1602);
        data.Game.Mates.PartnerSkills[1].SkillVNum.ShouldBe(1603);
        data.Game.Mates.PartnerSkills[2].SkillVNum.ShouldBe(1604);
        data.Game.Mates.PetSkill.ShouldNotBeNull();
        data.Game.Mates.PetSkill.SkillVNum.ShouldBe(663);
    }
}