//
//  NRunType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using NosSmooth.Packets.Client;

#pragma warning disable CS1591
namespace NosSmooth.Packets.Enums.NRun;

/// <summary>
/// A type of <see cref="NRunPacket"/> packet.
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Too many fields.")]
public enum NRunType
{
    TimesUp = 0,
    ChangeClass = 1,
    UpgradeItemNpc = 2,
    GetPartner = 3,
    Mate = 4,
    TimeSpaceTimer = 5,
    TimeSpaceEndDialog = 6,
    JewelryCellon = 10,
    OpenWindow = 12,
    OpenCrafting = 14,
    SetRevivalSpawn = 15,
    WarpTeleport = 16,
    AskEnterArena = 17,
    CircleTimeSkill = 18,
    TimeSpaceUnknown = 19,
    FamilyDialogue = 23,
    WarpTeleportAct5 = 26,
    GrasslinEvent = 31,
    Nos11YearMedalProduction = 36,
    NosProduction = 37,
    Year11Production = 38,
    RaidList = 39,
    EggFairyProduction = 40,
    GrasslinSealProduction = 41,
    WeddingGazebo = 45,
    RedMagicFairyProduction = 46,
    OpenNosbazaar = 60,
    GrenigasSealProduction = 61,
    TeleportGrenigasSquare = 62,
    IcyFlowersMission = 65,
    HeatPotionMission = 66,
    JohnMission = 67,
    AkamurMission = 68,
    IceFlowers10Production = 69,
    MagicCamelProduction = 70,
    MagicCamelBoxProduction = 71,
    TimespaceOnFinishDialog = 144,
    LandOfChaosEnter = 150,
    UseBank = 321,
    GetBankBook = 322,
    AmoraMission = 327,
    ShowPlayerShop = 900,
    EnterToTsId = 1000,
    DailyMissionTarot = 1500,

    /* HALLOWEEN */
    HalloweenProductionBox = 47,
    HalloweenProductionCostume10YellowCandy = 72,
    HalloweenProductionCostume10BlackCandy = 73,
    HalloweenProductionSeal30YellowCandy = 74,
    HalloweenProductionSeal30BlackCandy = 75,
    HalloweenProductionSealBagOfSweats = 76,
    HalloweenMissionMary = 77,
    HalloweenMissionEva = 78,
    HalloweenMissionMalcolm = 79,
    HalloweenMissionTeoman = 80,
    HalloweenMissionEric = 81,
    HalloweenCatacombsEntrance = 316,
    HalloweenProductionLaurenaWand = 317,

    /* WINTER */
    WinterProductionBox = 34,
    WinterProductionSnowmanSeal = 35,
    WinterMissionSanta = 82,
    WinterProductionSpecialBag = 83,
    WinterProductionSealedChristVessel30CreamCake = 84,
    WinterProductionCostume30ChocolateCake = 85,
    WinterProductionSeal30CreamCake = 86,
    WinterProductionSeal30ChocolateCake = 87,
    WinterProduction5StockingsChristBox = 88,
    WinterMissionSlugg = 89,
    WinterMissionEva = 90,
    WinterMissionMalcolm = 91,
    WinterMissionTeoman = 92,
    WinterMissionSoraya = 93,
    WinterMissionVikings = 128,
    WinterProduction10Happy = 129,
    WinterProduction10HappyYear = 130,
    WinterMissionFirstSonPark1 = 178,
    WinterMissionFirstSonPark2 = 179,
    WinterMissionSecondSonPark1 = 180,
    WinterMissionSecondSonPark2 = 181,
    WinterMissionThirdSonPark1 = 182,
    WinterMissionThirdSonPark2 = 183,
    WinterMissionWanderer1 = 184,
    WinterMissionWanderer2 = 185,
    WinterMissionWanderer3 = 186,
    WinterMissionMaru1 = 187,
    WinterMissionMaru2 = 188,
    WinterProduction3BrassCoinsBox = 190,
    WinterProductionBetterSleigh = 325,
    WinterMissionReindeer = 326,
    WinterProductionHappy = 6013,
    WinterProductionHappyYear = 6014,

    /* EASTER */
    EasterMissionMimi = 94,
    EasterProduction5GoldEggsBox = 95,
    EasterProduction30ChocolateQfcSeal = 96,
    EasterMissionSlugg = 97,
    EasterMissionCalvin = 98,
    EasterMissionEva = 99,
    EasterMissionMalcolm = 100,
    EasterMissionHungbu = 191,
    EasterMissionHungbuSwallow = 192,
    EasterProduction10CleanEggsCake = 328,
    EasterProduction10CleanEggsBox = 329,
    EasterProduction10RotterEggsClean = 330,
    EasterMissionSluggStory = 331,

    /* SP5 & SP6 MISSIONS/PRODUCTION */
    DracoAmuletMission = 110,
    GlacerusAmuletMission = 131,
    DracoSealProduction = 111,
    EnterToWatterGroto = 132,
    GlacerusSealProduction = 133,
    GiveSp6TsMission = 134,
    DracoClawSp5Production = 145,
    DracoClawSp5PerfStonProduction = 146,
    GlacerusManeSp6Production = 147,
    GlacerusManeSp6PerfStoneProduction = 148,

    /* ARENA OF TALENTS/MASTERS */
    ArenaOfTalentsRegistration = 135,
    ArenaOfMastersRegistration = 136,
    ArenaOfMastersSpectatorChoice = 137,
    ArenaOfTalentsSpectatorRandom = 138,
    ArenaOfMastersSpectatorRandom = 139,
    ArenaOfMastersRanking = 140,

    /* SP8 MISSIONS/PRODUCTION */
    Sp8MissionLaurena = 193,
    Sp8ProductionSoulSliver3Powder = 194,
    Sp8Production5SeedDamnationSeal = 195,

    /* SUMMER */
    SummerMissionRaphaelStory = 197,
    SummerProductionBunnyLiverWithBox = 198,
    SummerProductionBunnyLiver = 199,
    SummerMissionRaphael = 201,
    SummerMissionJack = 1507,
    SummerEventPirateSp = 1508,
    SummerProduction10Mariner10MapSeal = 1509,
    SummerProductionPirateSpLuck = 1511,

    /* ACT 6 */
    Act61MissionFirst = 300,
    Act61TeleportCylloan = 301,
    Act62MissionFirst = 302,
    Act62MissionHealingChoiceSave = 303,
    Act62MissionHealingChoiceNoSave = 304,
    Act62EnterToShip = 305,
    Act62EnterToShip2 = 306,
    Act62BackToCylloan = 307,

    /* VALHALLA */
    ValhallaMissionBarni = 308,
    ValhallaMissionRagnar10KkChoice = 309,
    ValhallaMissionRagnarFreeChoice = 310,
    ValhallaMissionRagnar = 311,
    ValhallaMissionFrigg = 312,
    ValhallaEnterToTartHap = 313,
    ValhallaBackToPort = 314,
    ValhallaMissionJennifer = 315,

    /* MARTIAL ARTIST */
    MaMissionSp2 = 324,
    MaMissionSp3 = 340,

    /* FAMILY */
    FamilyWarehouseOpen = 1600,
    FamilyWarehouseHistory = 1601,
    FamilyUpgradeWarehouse21 = 1602,
    FamilyUpgradeWarehouse49 = 1603,
    FamilyUpgradeMembersSize70 = 1604,
    FamilyUpgradeMembersSize100 = 1605,

    /* QUESTS */
    QuestAdditionalAct5 = 200,
    QuestAdditional = 2000,
    QuestEndSp = 2001,
    QuestTeleportToSpMap = 2002,
    QuestReceive = 3000,
    QuestReceiveMain = 3001,
    QuestReceiveTsPoints = 3002,
    QuestReceiveAdditionalBlue = 3006, // (\!/)

    /* ACT 4/5 */
    Act4EnterShip = 5001,
    Act4Leave = 5002,
    Act4LeaveShip = 5004,
    Act4Enter = 5005,
    Act5EnterShip = 5011,
    Act5Leave = 5012,
    Act5LeaveShip = 5014
}