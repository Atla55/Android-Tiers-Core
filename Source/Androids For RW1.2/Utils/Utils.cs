using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using HarmonyLib;
using Verse.AI.Group;
using System.Reflection;
using Verse.Sound;
using Verse.AI;

namespace MOARANDROIDS
{
    public static class Utils
    {

        public static bool init = false;
        public static Harmony harmonyInstance;

        public static List<WorkGiverDef> CrafterDoctorJob;

        public static bool PSYCHOLOGY_LOADED = false;
        public static bool HELLUNIT_LOADED = false;
        public static bool SMARTMEDICINE_LOADED = false;
        static public bool CELOADED = false;
        static public bool MEDICINEPATCH_LOADED = false;
        static public bool BIRDSANDBEES_LOADED = false;
        static public bool PRISONLABOR_LOADED = false;
        static public bool SAVEOURSHIP2_LOADED = false;
        static public bool WORKTAB_LOADED = false;
        static public bool HOSPITALITY_LOADED = false;
        static public bool SEARCHANDDESTROY_LOADED = false;
        static public bool FACTIONDISCOVERY_LOADED = false;
        static public bool POWERPP_LOADED = false;
        static public bool ANDROIDTIERSGYNOID_LOADED = false;
        static public bool QEE_LOADED = false;
        static public bool RIMMSQOL_LOADED = false;


        static public bool TXSERIE_LOADED = false;

        private static readonly FloatRange settlementsBasesPer100KTiles = new FloatRange(75f, 85f);

        public static Pawn curSelPatientDrawMedOperationsTab;

        public static int lastDoorOpenedVocalGT = 0;
        public static int lastDoorClosedVocalGT = 0;

        public static int lastDeviceActivatedVocalGT = 0;
        public static int lastDeviceDeactivatedVocalGT = 0;

        public static Pawn lastInstallImplantBillDoer;

        public static bool forceGeneratedAndroidToBeDefaultPainted = false;

        public static Assembly smartMedicineAssembly;
        public static Assembly medicinePatchAssembly;
        public static Assembly androidTiersAssembly;
        public static Assembly prisonLaborAssembly;
        public static Assembly saveOurShip2Assembly;
        public static Assembly hospitalityAssembly;
        public static Assembly searchAndDestroyAssembly;
        public static Assembly factionDiscoveryAssembly;
        public static Assembly powerppAssembly;
        public static Assembly qeeAssembly;


        public static string lastResolveAllGraphicsHeadGraphicPath = null;
        public static BodyTypeDef insideResolveApparelGraphicsLastBodyTypeDef;

        public static bool insideAddHumanlikeOrders = false;

        public static bool insideKillFuncSurrogate = false;
        public static string headGraphicPathToUse = "";

        public static bool lastButcheredPawnIsAndroid = false;
        public static int lastMemoryThoughtAgeBeforeReset = 0;

        public static int lastPlayedVocalWarningNoSkyMindNetGT = 0;


        public static float PawnInventoryGeneratorLastInvNutritionValue = 0;
        public static bool PawnInventoryGeneratorCanHackInvNutritionValue = true;

        public static bool InsideBestFoodSourceOnMap = false;

        public static bool preventVX0Thought = false;

        public static WorkTypeDef WorkTypeDefSmithing;

        public static ThoughtDef thoughtDefVX0Puppet;

        public static TraitDef traitSimpleMinded;

        public static HediffDef hediffHaveRXChip;
        public static HediffDef hediffHaveVX0Chip;
        public static HediffDef hediffHaveVX1Chip;
        public static HediffDef hediffHaveVX2Chip;
        public static HediffDef hediffHaveVX3Chip;
        public static HediffDef hediffNoHost;
        public static HediffDef hediffLowNetworkSignal;
        public static HediffDef hediffRusted;
        public static HediffDef hediffBlankAndroid;

        public static SoundDef soundDefSkyCloudMindQuarantineMentalState;
        public static SoundDef soundDefSkyCloudMindUploadCompleted;
        public static SoundDef soundDefSkyCloudMindDownloadCompleted;
        public static SoundDef soundDefSkyCloudDeviceActivated;
        public static SoundDef soundDefSkyCloudDeviceDeactivated;
        public static SoundDef soundDefSkyCloudDoorOpened;
        public static SoundDef soundDefSkyCloudDoorClosed;
        public static SoundDef soundDefSkyCloudSkyMindNetworkOffline;
        public static SoundDef soundDefSkyCloudPowerFailure;
        public static SoundDef soundDefSkyCloudMindReplicationCompleted;
        public static SoundDef soundDefSkyCloudMindMigrationCompleted;
        public static SoundDef soundDefSkyCloudMindDeletionCompleted;
        public static SoundDef soundDefSkyCloudAllMindDisconnected;
        public static SoundDef soundDefSkyCloudPrimarySystemsOnline;
        public static SoundDef soundDefSurrogateConnection;
        public static SoundDef soundDefSurrogateConnectionStopped;
        public static SoundDef soundDefTurretConnection;
        public static SoundDef soundDefTurretConnectionStopped;
        public static SoundDef soundDefSurrogateHacked;

        public static StatDef statDefAndroidTending;
        public static StatDef statDefAndroidSurgerySuccessChance;

        public static List<MentalBreakDef> VirusedRandomMentalBreak = new List<MentalBreakDef>();

        public static List<TraitDef> RansomAddedBadTraits = new List<TraitDef>();
        
        //public static Color SXColor = new Color(0.463f, 0.62f, 0.463f);
        public static Color SXColor = new Color(0.280f, 0.280f, 0.280f);

        public static Color androidCustomColorKhaki = new Color(0.29411f, 0.356862f, 0.16470f);
        public static Color androidCustomColorGreen = new Color(0.19f, 0.75f, 0.43f);
        public static Color androidCustomColorWhite = new Color(1.0f, 1.0f, 1.0f);
        public static Color androidCustomColorBlack = new Color(0.15f, 0.15f, 0.15f);
        public static Color androidCustomColorGray = new Color(0.50f, 0.50f, 0.50f);
        public static Color androidCustomColorBlue = new Color(0.25f, 0.44f, 0.69f);
        public static Color androidCustomColorRed = new Color(0.69f, 0.26f, 0.29f);
        public static Color androidCustomColorOrange = new Color(1.0f, 0.64705f, 0.0f);
        public static Color androidCustomColorYellow = new Color(0.8392f, 0.8274f, 0.1254f);
        public static Color androidCustomColorPurple = new Color(0.43f, 0.30f, 0.55f);
        public static Color androidCustomColorPink = new Color(0.90f, 0.60f, 0.83f);
        public static Color androidCustomColorCyan = new Color(0.33f, 0.69f, 0.83f);
        
        public static Color androidCustomColorRust = new Color(0.5607f, 0.2941f, 0.1764f);


        public static Pawn FindBestMedicinePatient = null;

        public const string TX2 = "ATPP_Android2TX";
        public const string TX3 = "ATPP_Android3TX";
        public const string TX4 = "ATPP_Android4TX";
        public const string TX2K = "ATPP_Android2KTX";

        public const string TX2I = "ATPP_Android2ITX";
        public const string TX3I = "ATPP_Android3ITX";
        public const string TX4I = "ATPP_Android4ITX";
        public const string TX2KI = "ATPP_Android2KITX";

        public const string T1 = "Android1Tier";
        public const string T2 = "Android2Tier";
        public const string T3 = "Android3Tier";
        public const string T4 = "Android4Tier";
        public const string T5 = "Android5Tier";
        public const string M7 = "M7Mech";
        public const string HU = "AT_HellUnit";
        public const string K9 = "AndroidDog";
        public const string MUFF = "AndroidMuff";
        public const string NSolution = "RoboticCow";
        public const string Phytomining = "RoboticSheep";
        public const string RoboticFennec = "AndroidFox";



        public static ThingDef M7Mech;

        public static List<string> BlacklistedHediffsForAndroids = new List<string> { "Anxiety" };

        public static HediffDef dummyHeddif;

        public static GC_ATPP GCATPP;

        public static Pawn ignoredPawnNotifications = null;

        public static List<PawnKindDef> AndroidsPKDNeutral = new List<PawnKindDef>();
        public static List<PawnKindDef> AndroidsXSeriePKDNeutral = new List<PawnKindDef>();
        public static List<PawnKindDef> AndroidsXISeriePKDNeutral = new List<PawnKindDef>();

        public static List<PawnKindDef> AndroidsPKDHostile = new List<PawnKindDef>();
        public static List<PawnKindDef> AndroidsXSeriePKDHostile = new List<PawnKindDef>();
        public static List<PawnKindDef> AndroidsXISeriePKDHostile = new List<PawnKindDef>();

        public static List<string> ExceptionCooler = new List<string> { "Cooler" };
        public static List<string> ExceptionHeater = new List<string> { "Heater" };
        //public static List<string> ExceptionTurrets = new List<string> { "Turret_MiniTurret", "Turret_Autocannon", "StandardTurretGun", "SniperTurretGun", "LazerTurretGun", "EnergyTurretGun" };

        public static List<string> ExceptionBlacklistedFactionNoSurrogate = new List<string> { "RRY_Yautja_JungleClan", "RRY_Yautja_BadBloodFaction", "RRY_Xenomorph", //AVP
        "ElderThing_Faction",  //ElderThings
        "Harrowed", //Harrowed
        "FriendlyConstruct","Crystal", //Crystalloid 
        "Imouto_Civil","ImoutoTribe"  //ImoutoTribe
        };


        public static List<string> ExceptionSkinColors = new List<string> { "Verylight", "Light", "Fair", "Midbrown", "Darkbrown","Verydark" };
        public static List<string> ExceptionHairColors = new List<string> { "Blond", "Black", "Auburn", "Grey", "Ginger", "White" };

        public static List<string> ExceptionBodyTypeDefnameAndroidWithSkinMale = new List<string> { "ATPP_BodyTypeMaleHurted22TX", "ATPP_BodyTypeMaleHurted12TX", "ATPP_BodyTypeMaleHurted12KTX", "ATPP_BodyTypeMaleHurted22KTX", "ATPP_BodyTypeMaleHurted13TX", "ATPP_BodyTypeMaleHurted23TX", "ATPP_BodyTypeMaleHurted14TX", "ATPP_BodyTypeMaleHurted24TX" };
        public static List<string> ExceptionBodyTypeDefnameAndroidWithSkinFemale = new List<string> { "ATPP_BodyTypeFemaleHurted12TX", "ATPP_BodyTypeFemaleHurted22TX", "ATPP_BodyTypeFemaleHurted22KTX", "ATPP_BodyTypeFemaleHurted12KTX", "ATPP_BodyTypeFemaleHurted13TX", "ATPP_BodyTypeFemaleHurted23TX", "ATPP_BodyTypeFemaleHurted14TX", "ATPP_BodyTypeFemaleHurted24TX" };
        public static List<string> ExceptionAutodoors = new List<string> { "Autodoor" };

        public static List<string> ExceptionSkyCloudCores = new List<string> { "ATPP_SkyCloudCore" };
        public static List<string> ExceptionQEEGS = new List<string> { "ATPP_GS_TX2KMale", "ATPP_GS_TX2KFemale", "ATPP_GS_TX3Male", "ATPP_GS_TX3Female", "ATPP_GS_TX4Male", "ATPP_GS_TX4Female"};

        public static List<string> ExceptionSkillServers = new List<string> { "ATPP_I500Skill", "ATPP_I300Skill", "ATPP_I100Skill" };
        public static List<string> ExceptionSecurityServers = new List<string> { "ATPP_I500Security", "ATPP_I300Security", "ATPP_I100Security" };
        public static List<string> ExceptionHackingServers = new List<string> { "ATPP_I500Hacking", "ATPP_I300Hacking", "ATPP_I100Hacking" };
        public static List<string> ExceptionSurrogatePodGuest = new List<string> { "ATPP_AndroidPodGuest", "AndroidOperationBedGuest" };
        public static List<string> ExceptionSurrogatePod = new List<string> { "ATPP_AndroidPod", "AndroidOperationBed" };
        public static List<string> ExceptionSurrogateM7Pod = new List<string> { "ATPP_AndroidPodMech" };
        public static string defNameOldSecurityServer = "ATPP_I100Security";
        public static string defNameBasicSecurityServer = "ATPP_I300Security";
        public static string defNameAdvancedSecurityServer = "ATPP_I500Security";

        public static string defNameOldHackingServer = "ATPP_I100Hacking";
        public static string defNameBasicHackingServer = "ATPP_I300Hacking";
        public static string defNameAdvancedHackingServer = "ATPP_I500Hacking";

        public static string defNameOldSkillServer = "ATPP_I100Skill";
        public static string defNameBasicSkillServer = "ATPP_I300Skill";
        public static string defNameAdvancedSkillServer = "ATPP_I500Skill";

        public static List<string> ExceptionBionicHaveFeet = new List<string> { "ARLeg", "HydraulicLeg", "MakeshiftRLeg", "BRLeg", "AR2Leg"};
        public static List<string> ExceptionBionicHaveHand = new List<string> { "MiningArm", "HydraulicArm", "MakeshiftRArm", "BRArm", "ARArm", "AR2Arm" };

        public static List<string> ExceptionPlayerStartingAndroidPawnKindList= new List<string> { "AndroidT1ColonistGeneral", "AndroidT2ColonistGeneral", "AndroidT3ColonistGeneral", "AndroidT4ColonistGeneral", "AndroidT5ColonistGeneral", "ATPP_Android2TXKind", "ATPP_Android3TXKind", "ATPP_Android4TXKind", "ATPP_Android2KTXKind", "ATPP_Android2KITXKind", "ATPP_Android2ITXKind", "ATPP_Android3ITXKind", "ATPP_Android4ITXKind" };

        public static List<string> ExceptionAndroidsDontRust = new List<string> { "ATPP_Android2TX", "ATPP_Android3TX", "ATPP_Android4TX", "ATPP_Android2KTX", "ATPP_Android2ITX", "ATPP_Android3ITX", "ATPP_Android4ITX", "ATPP_Android2KITX" };

        public static List<string> ExceptionTXSerie = new List<string> { "ATPP_Android2TX", "ATPP_Android3TX", "ATPP_Android4TX", "ATPP_Android2KTX", "ATPP_Android2ITX", "ATPP_Android3ITX", "ATPP_Android4ITX", "ATPP_Android2KITX" };

        public static List<string> ExceptionAndroidWithoutSkinList = new List<string> { };
        public static List<string> ExceptionAndroidWithSkinList = new List<string> { "ATPP_Android2TX", "ATPP_Android3TX", "ATPP_Android4TX", "ATPP_Android2KTX" };

        public static List<string> ExceptionNanoKits = new List<string> { "ATPP_AndroidNanokitBasic", "ATPP_AndroidNanokitIntermediate", "ATPP_AndroidNanokitAdvanced" };
        public static List<string> ExceptionRegularAndroidList = new List<string> { "Android1Tier", "Android2Tier", "Android3Tier", "Android4Tier", "Android5Tier", "AT_HellUnit", "ATPP_Android2TX", "ATPP_Android3TX", "ATPP_Android4TX", "ATPP_Android2KTX", "ATPP_Android2ITX", "ATPP_Android2KITX", "ATPP_Android3ITX", "ATPP_Android4ITX" };
        public static List<string> ExceptionAndroidList = new List<string> { "Android1Tier", "Android2Tier","Android3Tier","Android4Tier","Android5Tier", "M7Mech", "AT_HellUnit", "ATPP_Android2TX", "ATPP_Android3TX", "ATPP_Android4TX", "ATPP_Android2KTX", "ATPP_Android2ITX", "ATPP_Android2KITX", "ATPP_Android3ITX", "ATPP_Android4ITX" };
        public static List<string> ExceptionAndroidCorpseList = new List<string> { "Corpse_Android1Tier", "Corpse_Android2Tier", "Corpse_Android3Tier", "Corpse_Android4Tier", "Corpse_Android5Tier", "Corpse_AT_HellUnit","Corpse_M7Mech"};
        public static List<string> ExceptionAndroidListBasic = new List<string> { "Android1Tier", "Android2Tier", "M7Mech", "ATPP_Android2TX", "ATPP_Android2KTX", "ATPP_Android2ITX" };
        public static List<string> ExceptionAndroidListAdvanced = new List<string> { "Android3Tier", "Android4Tier", "Android5Tier", "AT_HellUnit", "ATPP_Android3TX", "ATPP_Android4TX", "ATPP_Android3ITX", "ATPP_Android4ITX" };
        public static List<string> ExceptionAndroidAnimalPowered = new List<string> { "AndroidMuff", "AndroidDog", "RoboticSheep", "RoboticCow", "AndroidFox" };
        public static List<string> ExceptionAndroidAnimals = new List<string> { "AndroidMuff", "AndroidDog", "RoboticSheep", "RoboticCow", "AndroidChicken", "AndroidFox" };

        public static List<string> BlacklistAndroidHediff = new List<string> { "VacuumDamageHediff", "ZeroGSickness", "SpaceHypoxia", "ClinicalDeathAsphyxiation", "ClinicalDeathNoHeartbeat", "FatalRad", "RimatomicsRadiation", "RadiationIncurable" };
        public static List<string> BlacklistMindTraits = new List<string> { "NightOwl", "Insomniac", "Codependent", "HeavySleeper", "Polygamous", "Beauty", "Immunity" };
        public static List<string> BlacklistAndroidFood = new List<string> { "SmokeleafJoint", "Yayo", "PsychiteTea", "Flake", "Penoxycyline", "Luciferium", "GoJuice", "Ambrosia", "Beer", "RC2_Coffee", "" };

        public static List<string> ExceptionNeuralChip = new List<string> { "ATPP_HediffVX3Chip", "ATPP_HediffVX2Chip", "ATPP_HediffVX1Chip", "ATPP_HediffVX0Chip" };

        public static List<string> ExceptionVXNeuralChipSurgery = new List<string> { "ATPP_InstallVX0ChipOnAndroid", "ATPP_InstallVX0Chip", "ATPP_InstallVX1ChipOnAndroid", "ATPP_InstallVX1Chip", "ATPP_InstallVX2ChipOnAndroid", "ATPP_InstallVX2Chip", "ATPP_InstallVX3ChipOnAndroid", "ATPP_InstallVX3Chip" };
        public static List<string> ExceptionArtificialBrainsSurgery = new List<string> { "ATPP_InstallT1ArtificialBrain", "ATPP_InstallT2ArtificialBrain", "ATPP_InstallT3ArtificialBrain", "ATPP_InstallT4ArtificialBrain" };

        public static List<string> ExceptionAndroidOnlyHediffs = new List<string> { "PlatingSteel", "PlatingPlasteel", "PlatingComposite", "MiningArm", "HydraulicFrame", "HydraulicLeg",
            "HydraulicArm", "MakeshiftRLeg", "MakeshiftRArm","ALReceptor","ARLeg","AHeatsink","ACoolantPump","ABattery","AMStorage","ATransformer","AVAdapter","AdvRearCounterweight","AdvJawAndroid",
            "BRArm","BRLeg","BLReceptor","BHeatsink","BBattery","BMStorage","BasicRearCounterweight","CrudeJawAndroid","BCoolantPump","BTransformer","BVAdapter","ARArm","PositronMind",
        "SoldierMind","BuilderMind","SurgeonMind","SpeedMind","GeoMind","MechanicMind","CookingMind","CharismaMind","NegotiatorMind","ZoologyMind","AgriculturalMind","UnskilledMind","AR2Arm","AR2Leg","AL2Receptor","A2Heatsink",
        "A2CoolantPump","A2Battery","A2MStorage","A2Transformer","A2VAdapter","HearingSensorCrude","HearingSensorAdv","HearingSensorArch","SmellSensorAdv","EvolvingMind"};

        public static List<string> ExceptionAndroidCanReloadWithPowerList;


        public static List<string> AndroidOldAgeHediffCPU = new List<string> { "CorruptMemory"};
        public static List<string> AndroidOldAgeHediffCooling = new List<string> { "ExaggeratedHealing" };
        public static List<string> AndroidOldAgeHediffFramework = new List<string> { "DecayedFrame" };
        public static List<string> AndroidOldAgeHediffHydraulic = new List<string> { "FaultyPump", "WeakValves" };

        public static List<string> IgnoredThoughtsByAllAndroids = new List<string> { "SoakingWet", "EnvironmentCold", "AteWithoutTable", "EnvironmentHot", "SleptInCold", "SleptInHeat", "AteRawFood", "AteAwfulMeal", "AteKibble", "AteInsectMeatDirect", "AteInsectMeatAsIngredient" };
        public static List<string> IgnoredThoughtsByBasicAndroids = new List<string> { "KnowColonistOrganHarvested", "ColonistBanished", "WasImprisoned", "KnowGuestOrganHarvested","KnowPrisonerSold","AteRottenFood","BondedAnimalBanished", "ColonistBanishedToDie", "ButcheredHumanlikeCorpse", "PrisonerBanishedToDie","KnowButcheredHumanlikeCorpse","EnvironmentDark", "ApparelDamaged", "DeadMansApparel",
            "HumanLeatherApparelSad", "HumanLeatherApparelHappy","SoldPrisoner", "ExecutedPrisoner", "KilledColonyAnimal","SleptOutside",
            "SleptOnGround","KnowGuestExecuted","KnowColonistExecuted","KnowColonistDied","WitnessedDeathAlly","WitnessedDeathNonAlly",
            "WitnessedDeathFamily","WitnessedDeathBloodlust","KilledHumanlikeBloodlust","PawnWithGoodOpinionDied","PawnWithBadOpinionDied",
            "AteCorpse","ObservedLayingCorpse", "ObservedLayingRottingCorpse", "AteHumanlikeMeatDirect","AteHumanlikeMeatDirectCannibal","AteHumanlikeMeatAsIngredient","AteHumanlikeMeatAsIngredientCannibal","ATPP_VX0PuppetThought" };

        public static List<string> IgnoredInteractionsByBasicAndroids = new List<string> { "RomanceAttempt", "MarriageProposal", "Breakup"};


        public static List<HediffDef> ExceptionRepairableFrameworkHediff;

        public static ResearchProjectDef ResearchProjectSkyMindLAN;
        public static ResearchProjectDef ResearchProjectSkyMindWAN;

        public static ResearchProjectDef ResearchAndroidBatteryOverload;


        public static int getConsumedPowerByAndroid(string defName)
        {
            int ret = 0;
            switch (defName)
            {
                case Utils.T1:
                    ret = Settings.wattConsumedByT1;
                    break;
                case Utils.TX2:
                case Utils.TX2I:
                case Utils.TX2K:
                case Utils.TX2KI:
                case Utils.T2:
                    ret = Settings.wattConsumedByT2;
                    break;
                case Utils.TX3:
                case Utils.TX3I:
                case Utils.T3:
                    ret = Settings.wattConsumedByT3;
                    break;
                case Utils.TX4:
                case Utils.TX4I:
                case Utils.T4:
                    ret = Settings.wattConsumedByT4;
                    break;
                case Utils.T5:
                    ret = Settings.wattConsumedByT5;
                    break;
                case Utils.M7:
                    ret = Settings.wattConsumedByM7;
                    break;
                case Utils.HU:
                    ret = Settings.wattConsumedByHellUnit;
                    break;
                case Utils.K9:
                    ret = Settings.wattConsumedByK9;
                    break;
                case Utils.MUFF:
                    ret = Settings.wattConsumedByMUFF;
                    break;
                case Utils.Phytomining:
                    ret = Settings.wattConsumedByPhytomining;
                    break;
                case Utils.NSolution:
                    ret = Settings.wattConsumedByNSolution;
                    break;
                case Utils.RoboticFennec:
                    ret = Settings.wattConsumedByFENNEC;
                    break;
            }

            return ret;
        }


        //Try give available android pod on map pwoerOn
        public static Building_Bed getAvailableAndroidPodForCharging(Pawn android, bool M7=false)
        {
            Map map = android.Map;
            Building_Bed ret = null;
            float dist = -1;
            //bool alreadyOwnBed = false;

            if(android.ownership != null && android.ownership.OwnedBed != null)
            {
                //alreadyOwnBed = true;
                if (((!M7 && android.ownership.OwnedBed.def.defName == "ATPP_AndroidPod")
                    || (M7 && android.ownership.OwnedBed.def.defName == "ATPP_AndroidPodMech"))
                    && android.CanReserveAndReach(android.ownership.OwnedBed, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, false)
                    && android.ownership.OwnedBed.Position.InAllowedArea(android)
                    && !android.ownership.OwnedBed.Destroyed && android.ownership.OwnedBed.TryGetComp<CompPowerTrader>() != null && android.ownership.OwnedBed.TryGetComp<CompPowerTrader>().PowerOn)
                {
                    return android.ownership.OwnedBed;
                }

                return null;
            }

            foreach(var el in map.listerBuildings.allBuildingsColonistElecFire)
            {
                //Selection android pod valide et alimenté
                if(((!M7 && el.def.defName == "ATPP_AndroidPod") || (M7 && el.def.defName == "ATPP_AndroidPodMech"))
                && !el.Destroyed && el.TryGetComp<CompPowerTrader>() != null)
                {
                    Building_Bed bed = (Building_Bed)el;

                    if (!bed.Medical
                    && (android.IsPrisoner == bed.ForPrisoners)
                    && !(bed.GetCurOccupant(0) != null || (bed.OwnersForReading.Count() != 0 && !bed.OwnersForReading.Contains(android)))
                    && el.TryGetComp<CompPowerTrader>().PowerOn
                    && el.Position.InAllowedArea(android)
                    && android.CanReserveAndReach(el, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, false))
                    {
                        float cdist = android.Position.DistanceTo(el.Position);

                        if (dist == -1 || cdist < dist)
                        {
                            ret = (Building_Bed)el;
                            dist = cdist;
                        }
                    }
                    //break;
                }
            }

            
            //Si android possede pas de lit et pas un lit medical on lui affecte le lit 
            if(ret != null)
            {
                android.ownership.ClaimBedIfNonMedical(ret);
            }

            return ret;
        }

        public static void playVocal(string vocal)
        {
            if (!GCATPP.isThereSkyCloudCore() || Settings.disableAbilitySkyCloudServerToTalk)
                return;

            int CGT = Find.TickManager.TicksGame;

            switch (vocal)
            {
                case "soundDefSkyCloudMindQuarantineMentalState":
                    Utils.soundDefSkyCloudMindQuarantineMentalState.PlayOneShot(null);
                    break;
                case "soundDefSkyCloudMindDownloadCompleted":
                    Utils.soundDefSkyCloudMindDownloadCompleted.PlayOneShot(null);
                    break;
                case "soundDefSkyCloudMindUploadCompleted":
                    Utils.soundDefSkyCloudMindUploadCompleted.PlayOneShot(null);
                    break;
                case "soundDefSkyCloudDoorOpened":
                    if ((CGT - Utils.lastDoorOpenedVocalGT) >= 300)
                    {
                        Utils.soundDefSkyCloudDoorOpened.PlayOneShot(null);
                        Utils.lastDoorOpenedVocalGT = CGT;
                    }
                    break;
                case "soundDefSkyCloudDoorClosed":
                    if ((CGT - lastDoorClosedVocalGT) >= 300)
                    {
                        soundDefSkyCloudDoorClosed.PlayOneShot(null);
                        lastDoorClosedVocalGT = CGT;
                    }
                    break;
                case "soundDefSkyCloudSkyMindNetworkOffline":
                    if ((CGT - lastPlayedVocalWarningNoSkyMindNetGT) >= 900)
                    {
                        Utils.soundDefSkyCloudSkyMindNetworkOffline.PlayOneShot(null);
                        lastPlayedVocalWarningNoSkyMindNetGT = CGT;
                    }
                    break;
                case "soundDefSkyCloudMindReplicationCompleted":
                    Utils.soundDefSkyCloudMindReplicationCompleted.PlayOneShot(null);
                    break;
                case "soundDefSkyCloudMindMigrationCompleted":
                    Utils.soundDefSkyCloudMindMigrationCompleted.PlayOneShot(null);
                    break;
                case "soundDefSkyCloudPrimarySystemsOnline":
                    Utils.soundDefSkyCloudPrimarySystemsOnline.PlayOneShot(null);
                    break;
                case "soundDefSkyCloudAllMindDisconnected":
                    Utils.soundDefSkyCloudAllMindDisconnected.PlayOneShot(null);
                    break;
                case "soundDefSkyCloudMindDeletionCompleted":
                    Utils.soundDefSkyCloudMindDeletionCompleted.PlayOneShot(null);
                    break;
                case "soundDefSkyCloudPowerFailure":
                    Utils.soundDefSkyCloudPowerFailure.PlayOneShot(null);
                    break;
                case "soundDefSkyCloudDeviceDeactivated":
                    if ((CGT - Utils.lastDeviceActivatedVocalGT) > 300)
                    {
                        Utils.soundDefSkyCloudDeviceDeactivated.PlayOneShot(null);
                        Utils.lastDeviceActivatedVocalGT = CGT;
                    }
                    break;
                case "soundDefSkyCloudDeviceActivated":
                    if ((CGT - Utils.lastDeviceDeactivatedVocalGT) > 300)
                    {
                        Utils.soundDefSkyCloudDeviceActivated.PlayOneShot(null);
                        Utils.lastDeviceDeactivatedVocalGT = CGT;
                    }
                    break;

            }
        }


        public static bool DynamicMedicalCareSetterPrefixPostfix(Rect rect, ref MedicalCareCategory medCare)
        {
            if (_DynamicMedicalCareSetter_NoAndroidSelected())
            {
                return true;
            }
            else
            {
                //Android selectionné on affiche les infos de medecine du mod
                MedicalCareUtility_Patch.MedicalCareSetter_Patch.Listener(rect, ref medCare);
                return false;
            }
        }


        public static bool DynamicMedicalCareSetterPrefix(Rect rect, ref MedicalCareCategory medCare)
        {
            return _DynamicMedicalCareSetter_NoAndroidSelected();
        }

        public static bool _DynamicMedicalCareSetter_NoAndroidSelected()
        {
            List<object> obj = Find.Selector.SelectedObjects;
            try
            {
                if (obj.Count == 1 && (obj[0] is Pawn))
                {
                    Pawn pawn = (Pawn)obj[0];

                    if (Utils.ExceptionAndroidList.Contains(pawn.def.defName))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }


        public static bool FindBestMedicinePrefix(Pawn healer, Pawn patient, out int totalCount)
        {
            totalCount = 0;
            //Log.Message("LOL !!! ICI");
            Utils.FindBestMedicinePatient = patient;
            return true;
        }

        public static void FindBestMedicinePostfix(Pawn healer, Pawn patient, out int totalCount)
        {
            totalCount = Medicine.GetMedicineCountToFullyHeal(patient);
            Utils.FindBestMedicinePatient = null;
        }

        public static void genericPostFixExtraCrafterDoctorJobs(Pawn pawn, Thing t, bool forced, ref bool __result, WorkGiver __instance)
        {
            if (!Settings.androidsCanOnlyBeHealedByCrafter)
            {
                //Si les jobs fake de docteur en mode crafter on les jertes
                if (__instance.def.workType == Utils.WorkTypeDefSmithing && Utils.CrafterDoctorJob.Contains(__instance.def))
                {
                    __result = false;
                }

                return;
            }

            //Medecin normal on jerte si t est un android
            if (__instance.def.workType == WorkTypeDefOf.Doctor)
            {
                if (t is Pawn && ((Pawn)t).IsAndroidTier())
                    __result = false;
            }
            else
            {
                if (Utils.CrafterDoctorJob.Contains(__instance.def))
                {
                    //Crafteur on jerte si patient pas un android
                    if (t is Pawn && ((Pawn)t).IsAndroidTier())
                    {
                        CompSurrogateOwner cso = pawn.TryGetComp<CompSurrogateOwner>();

                        if (cso == null || !cso.repairAndroids)
                            __result = false;
                    }
                    else
                        __result = false;
                }
            }
        }

        public static Map getMapFromString(this string MUID)
        {
            foreach(var m in Find.Maps)
            {
                if (m.GetUniqueLoadID() == MUID)
                    return m;
            }

            return null;
        }

        public static void removeAllSlowNetworkHediff(bool onlyInCaravan=false)
        {
            if (Current.ProgramState != ProgramState.Playing)
                return;

            if (!onlyInCaravan)
            {
                foreach (var m in Find.Maps)
                {
                    foreach (var p in m.mapPawns.FreeColonistsAndPrisoners)
                    {
                        if (p.IsSurrogateAndroid())
                        {
                            Hediff he = p.health.hediffSet.GetFirstHediffOfDef(Utils.hediffLowNetworkSignal);
                            if (he != null)
                            {
                                p.health.RemoveHediff(he);
                                //p.health.hediffSet.hediffs.Remove(he);
                                //he.PostRemoved();
                            }
                        }
                    }
                }
            }

            //Traitement des caravanes
            foreach(var e in Find.WorldObjects.Caravans)
            {
                foreach(var p in e.pawns)
                {
                    if (p.IsSurrogateAndroid())
                    {
                        Hediff he = p.health.hediffSet.GetFirstHediffOfDef(Utils.hediffLowNetworkSignal);
                        if (he != null)
                            p.health.RemoveHediff(he);
                    }
                }
            }
        }

        public static void throwChargingMote(Pawn cp)
        {
            if (cp.needs.food.CurLevelPercentage < 1.0)
            {
                //Envois d'une mote
                if (!cp.Map.moteCounter.Saturated)
                {
                    if (cp.needs.food.CurLevelPercentage >= 0.80f)
                        Utils.throwMote(DefDatabase<ThingDef>.GetNamed("ATPP_MoteBIII", true), cp);
                    else if (cp.needs.food.CurLevelPercentage >= 0.40f)
                        Utils.throwMote(DefDatabase<ThingDef>.GetNamed("ATPP_MoteBII", true), cp);
                    else
                        Utils.throwMote(DefDatabase<ThingDef>.GetNamed("ATPP_MoteBI", true), cp);
                }
            }
        }

        public static Lord LordOnMapWhereFactionIsInvolved(Map map, Faction faction)
        {
            foreach(var l in map.lordManager.lords)
            {
                if (l.faction == faction)
                    return l;
            }

            return null;
        }

        //public static List<string> RelationsException = new List<string> { "Bond" , "Lover", "Fiance", "Stepparent", "Stepchild", "ParentInLaw", "ChildInLaw", "ExSpouse", "ExLover", "Spouse" };

        public static Map getRandomMapOfPlayer()
        {
            Map ret = null;
            foreach (var map in Find.Maps)
            {
                if (map.IsPlayerHome)
                {
                    ret = map;
                    break;
                }
            }
            return ret;
        }


        //Restauration d'un nom sauvegarder
        public static void restoreSavedSurrogateName(Pawn surrogate)
        {
            CompAndroidState cas = surrogate.TryGetComp<CompAndroidState>();

            if (cas == null)
                return;

            string[] tmp = cas.savedName.Split('§');

            if (tmp.Count() != 3)
                return;

            surrogate.Name = new NameTriple(tmp[0], tmp[1], tmp[2]);
        }

        public static string getSavedSurrogateNameNick(Pawn surrogate)
        {
            CompAndroidState cas = surrogate.TryGetComp<CompAndroidState>();

            if (cas == null)
                return "";

            string[] tmp = cas.savedName.Split('§');

            if (tmp.Count() != 3)
                return "";

            return tmp[1];
        }

        public static void saveSurrogateName(Pawn surrogate)
        {
            CompAndroidState cas = surrogate.TryGetComp<CompAndroidState>();

            if (cas == null)
                return;

            NameTriple name = (NameTriple) surrogate.Name;
            cas.savedName = name.First + "§" + name.Nick + "§" + name.Last;
        }


        public static void initBodyAsSurrogate(Pawn surrogate, bool addSimpleMinded =true)
        {
            //SKills vierges
            surrogate.skills = new Pawn_SkillTracker(surrogate);
            surrogate.needs = new Pawn_NeedsTracker(surrogate);

            surrogate.story.traits.allTraits.Clear();

            //TOuts les SX sont simple minded et ont aucuns autres traits
            if (addSimpleMinded)
            {
                TraitDef td = DefDatabase<TraitDef>.GetNamed("SimpleMindedAndroid", false);
                Trait t = null;
                if (td != null)
                    t = new Trait(td);

                if (t != null)
                    surrogate.story.traits.allTraits.Add(t);
            }

            Utils.notifTraitsChanged(surrogate);

            //S'il sagit d'un M7 on vire sa batterie à la con
            if (surrogate.kindDef.defName == "M7MechPawn")
            {
                Hediff he = surrogate.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("BatteryChargeMech", false));
                if (he != null)
                {
                    surrogate.health.RemoveHediff(he);
                }
            }
        }

        public static Pawn generateSurrogate(Faction faction, PawnKindDef kindDef, IntVec3 pos, Map map, bool spawn=false, bool external=false, int tile=-1,bool allowFood=true, bool inhabitant=false, int Gender = -1)
        {
            Gender gender = Verse.Gender.Male;
            if(Gender != -1)
            {
                if (Gender == 0)
                    gender = Verse.Gender.Male;
                else
                    gender = Verse.Gender.Female;
            }

            PawnGenerationRequest request = new PawnGenerationRequest(kindDef, faction, PawnGenerationContext.NonPlayer, tile, false, false, false, false, true, true, 1f, false, true, allowFood, false,inhabitant, false, false, fixedGender : gender);
            Pawn surrogate = PawnGenerator.GeneratePawn(request);
            if (spawn)
                GenSpawn.Spawn(surrogate, pos, map, WipeMode.Vanish);

            initBodyAsSurrogate(surrogate);

            setSurrogateName(surrogate, external);


            //On va le définir comme étant un surrogate 
            CompAndroidState cas = surrogate.TryGetComp<CompAndroidState>();
            if (cas != null)
            {
                cas.initAsSurrogate();
            }

            return surrogate;
        }

        public static void setSurrogateName(Pawn surrogate, bool external=false )
        {
            int SXVer;
            string prefix = "";

            if (surrogate.def.defName == Utils.T1)
                SXVer = 1;
            else if (surrogate.def.defName == Utils.T2)
                SXVer = 2;
            else if (surrogate.def.defName == Utils.T3)
                SXVer = 3;
            else if (surrogate.def.defName == Utils.T4)
                SXVer = 4;
            else if (surrogate.def.defName == Utils.HU)
                SXVer = 10;
            else if (surrogate.def.defName == Utils.TX2 || surrogate.def.defName == Utils.TX2I)
            {
                SXVer = 12;
                prefix = "X";
            }
            else if (surrogate.def.defName == Utils.TX2K || surrogate.def.defName == Utils.TX2KI)
            {
                SXVer = 120;
                prefix = "X";
            }
            else if (surrogate.def.defName == Utils.TX3 || surrogate.def.defName == Utils.TX3I)
            {
                SXVer = 13;
                prefix = "X";
            }
            else if (surrogate.def.defName == Utils.TX4 || surrogate.def.defName == Utils.TX4I)
            {
                SXVer = 14;
                prefix = "X";
            }
            else if (surrogate.def.defName == "M7Mech")
            {
                prefix = "M";
                SXVer = 7;
            }
            else
                SXVer = 0;

            if (!external)
            {
                //On définis un nom séquentiel
                surrogate.Name = new NameTriple("", "S" + prefix + SXVer + "-" + Utils.GCATPP.getNextSXID(SXVer), "");
                Utils.GCATPP.incNextSXID(SXVer);
            }
            else
            {
                surrogate.Name = new NameTriple("", "S" + SXVer + "-" + Rand.Range(50, 1000), "");
            }
        }

        public static string TranslateTicksToTextIRLSeconds(int ticks)
        {
            //Si moins d'une heure ingame alors affichage secondes
            if (ticks < 2500)
                return ticks.ToStringSecondsFromTicks();
            else
                return ticks.ToStringTicksToPeriodVerbose(true);
        }


        public static bool isThereSolarFlare()
        {
            foreach(var map in Find.Maps)
            {
                if (map.IsPlayerHome && map.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare))
                    return true;
            }

            return false;
        }

        public static bool anyPlayerColonnyHasEnoughtSilver(int price)
        {
            foreach (var map in Find.Maps)
            {
                if (map.IsPlayerHome && TradeUtility.ColonyHasEnoughSilver(map, price))
                {
                    return true;
                }
            }
            return false;
        }

        public static void anyPlayerColonnyPaySilver(int price)
        {
            foreach (var map in Find.Maps)
            {
                if (map.IsPlayerHome && TradeUtility.ColonyHasEnoughSilver(map, price))
                {
                    TradeUtility.LaunchSilver(map, price);
                    break;
                }
            }
        }

        public static bool antennaSelected()
        {
            foreach(var el in Find.Selector.SelectedObjects)
            {
                if(el is Building)
                {
                    Building b = (Building)el;
                    if(b.def.defName == "ATPP_SkyMindLAN" || b.def.defName == "ATPP_SkyMindWAN")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool anySkyMindNetResearched()
        {
            return ResearchProjectSkyMindLAN.IsFinished || ResearchProjectSkyMindWAN.IsFinished;
        }

        public static void refreshHediff(Pawn pawn)
        {
            pawn.health.summaryHealth.Notify_HealthChanged();
            pawn.health.capacities.Notify_CapacityLevelsDirty();

            if (pawn.health.hediffSet != null)
            {
                for (int i = 0; i < pawn.health.hediffSet.hediffs.Count; i++)
                {
                    Hediff h = pawn.health.hediffSet.hediffs[i];
                    pawn.health.Notify_HediffChanged(h);
                }
            }
            pawn.health.hediffSet.DirtyCache();
        }

        public static bool AnyPressed(this Widgets.DraggableResult result)
        {
            return result == Widgets.DraggableResult.Pressed || result == Widgets.DraggableResult.DraggedThenPressed;
        }

        public static bool ContainsAny(this string haystack, List<string> needles)
        {
            foreach (string needle in needles)
            {
                if (haystack == needle)
                    return true;
            }

            return false;
        }


        public static void throwMote(ThingDef moteDef,Pawn android)
        {
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(moteDef, null);
            moteThrown.Scale = 0.6f;
            moteThrown.rotationRate = (float)Rand.Range(-1, 1);
            moteThrown.exactPosition = android.Position.ToVector3();
            moteThrown.exactPosition += new Vector3(0.85f, 0f, 0.85f);
            moteThrown.exactPosition += new Vector3(Rand.Value, 0f, Rand.Value) * 0.1f;
            moteThrown.SetVelocity(Rand.Range(30f, 60f), Rand.Range(0.35f, 0.55f));
            GenSpawn.Spawn(moteThrown, android.Position, android.Map, WipeMode.Vanish);
        }

        public static bool IsAndroidGen(this Pawn pawn)
        {
            return pawn.RaceProps.FleshType.defName == "Android" || pawn.RaceProps.FleshType.defName == "MechanisedInfantry" || pawn.RaceProps.FleshType.defName == "ChJDroid" || pawn.def.defName == "ChjAndroid";
        }

        public static bool IsAndroidTier(this Pawn pawn)
        {
            return Utils.ExceptionAndroidList.Contains(pawn.def.defName) ;
        }

        public static bool IsCyberAnimal(this Pawn pawn)
        {
            return Utils.ExceptionAndroidAnimals.Contains(pawn.def.defName);
        }

        public static bool IsBasicAndroidTier(this Pawn pawn)
        {
            return Utils.ExceptionAndroidListBasic.Contains(pawn.def.defName);
        }

        public static bool IsPoweredAnimalAndroids(this Pawn pawn)
        {
            return Utils.ExceptionAndroidAnimalPowered.Contains(pawn.def.defName);
        }

        public static Hediff HaveNotStackableVXChip(this Pawn pawn)
        {
            foreach(var h in pawn.health.hediffSet.hediffs)
            {
                if (ExceptionNeuralChip.Contains(h.def.defName))
                    return h;
            }

            return null;
        }

        public static bool IsSurrogateAndroid(this Pawn pawn, bool usedSurrogate=false, bool notUsedSurrogate = false)
        {
            /*if(!pawn.IsAndroidTier())
                return false;*/

            CompAndroidState cas = pawn.TryGetComp<CompAndroidState>();
            if (cas == null)
                return false;

            return cas.isSurrogate && (!usedSurrogate || cas.surrogateController != null) && (!notUsedSurrogate || cas.surrogateController == null);
        }

        public static bool IsBlankAndroid(this Pawn pawn)
        {
            CompAndroidState cas = pawn.TryGetComp<CompAndroidState>();
            if (cas == null)
                return false;

            return cas.isBlankAndroid;
        }

        public static bool haveAndroidOldAgeHediff(this Pawn pawn, List<string> issues)
        {
            foreach(var h in pawn.health.hediffSet.hediffs)
            {
                if (issues.Contains(h.def.defName))
                    return true;
            }

            return false;
        }

        public static bool VXAndVX0ChipPresent(this Pawn pawn)
        {
            return (pawn.VXChipPresent() || pawn.VX0ChipPresent());
        }

        public static bool VXChipPresent(this Pawn pawn)
        {
            return ( pawn.VX1ChipPresent() || pawn.VX2ChipPresent() || pawn.VX3ChipPresent() );
        }

        public static bool VX0ChipPresent(this Pawn pawn)
        {
            return pawn.health.hediffSet.GetFirstHediffOfDef(Utils.hediffHaveVX0Chip) != null;
        }

        public static bool VX1ChipPresent(this Pawn pawn)
        {
            return pawn.health.hediffSet.GetFirstHediffOfDef(Utils.hediffHaveVX1Chip) != null;
        }

        public static bool VX2ChipPresent(this Pawn pawn)
        {
            return pawn.health.hediffSet.GetFirstHediffOfDef(Utils.hediffHaveVX2Chip) != null;
        }

        public static bool VX3ChipPresent(this Pawn pawn)
        {
            return pawn.health.hediffSet.GetFirstHediffOfDef(Utils.hediffHaveVX3Chip) != null;
        }

        public static void showFailedLetterMindUpload(string reason)
        {
            Find.LetterStack.ReceiveLetter("ATPP_LetterInterruptedUpload".Translate(), "ATPP_LetterInterruptedUploadDesc".Translate(reason), LetterDefOf.ThreatSmall);
        }


        public static void ShowFloatMenuAndroidCandidate(Pawn emitter, Action<Pawn> onClick)
        {
            List<FloatMenuOption> opts = new List<FloatMenuOption>();
            FloatMenu floatMenuMap;

            //Listing des colons sauf ceux présents dans la liste d'exception (exp)
            foreach (var colon in emitter.Map.mapPawns.FreeColonistsAndPrisoners)
            {
                //SI colon vivant et relié au RimNet et pas dans la liste d'exception et possede une PUCE RIMNET
                if (colon != emitter && !colon.Dead
                    && GCATPP.isConnectedToSkyMind(colon)
                    && !colon.Destroyed && colon.IsAndroid() && !colon.IsSurrogateAndroid())
                {
                    CompAndroidState cab = colon.TryGetComp<CompAndroidState>();

                    //Les androids déjà en cours de transfert sont ignorés
                    if (cab.showUploadProgress || cab.uploadEndingGT != -1)
                        continue;

                    opts.Add(new FloatMenuOption(colon.LabelShortCap, delegate
                    {
                        onClick(colon);
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
            }
            opts.SortBy((x) => x.Label);

            //SI pas choix affichage de la raison 
            if (opts.Count == 0)
                opts.Add(new FloatMenuOption("ATPP_ConsciousnessUploadNoRecipient".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null));

            floatMenuMap = new FloatMenu(opts, "ATPP_ConsciousnessSelectDestination".Translate());
            Find.WindowStack.Add(floatMenuMap);
        }


        public static void ShowFloatMenuPermuteOrDuplicateCandidate(Pawn emitter, Action<Pawn> onClick,bool excludeBlankAndroid=false)
        {
            List<FloatMenuOption> opts = new List<FloatMenuOption>();
            FloatMenu floatMenuMap;

            //Listing des colons sauf ceux présents dans la liste d'exception (exp)
            foreach (var colon in emitter.Map.mapPawns.FreeColonistsAndPrisoners)
            {
                CompAndroidState cas = colon.TryGetComp<CompAndroidState>();

                //SI colon vivant et relié au RimNet et pas dans la liste d'exception et possede une PUCE RIMNET
                if (colon != emitter && !colon.Dead
                    && (colon.VX2ChipPresent() || colon.VX3ChipPresent())
                    && (!excludeBlankAndroid || (cas != null && !cas.isBlankAndroid))
                    && !colon.Destroyed && Utils.GCATPP.isConnectedToSkyMind(colon) && !colon.IsSurrogateAndroid())
                {
                    CompSkyMind csm = colon.TryGetComp<CompSkyMind>();

                    //Les androides infectés sont squeezés
                    if (csm != null && csm.Infected != -1)
                        continue;

                    CompSurrogateOwner cso = colon.TryGetComp<CompSurrogateOwner>();

                    //Les colons déjà en cours de transfert sont ignorés
                    if (cso.duplicateEndingGT != -1 || cso.permuteEndingGT != -1 || cso.showPermuteProgress || cso.showDuplicateProgress || (cso.controlMode && cso.isThereSX()))
                        continue;

                    opts.Add(new FloatMenuOption(colon.LabelShortCap, delegate
                    {
                        onClick(colon);
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
            }
            opts.SortBy((x) => x.Label);

            //SI pas choix affichage de la raison 
            if (opts.Count == 0)
                opts.Add(new FloatMenuOption("ATPP_NoVX2Recipient".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null));

            floatMenuMap = new FloatMenu(opts, "ATPP_ConsciousnessSelectDestination".Translate());
            Find.WindowStack.Add(floatMenuMap);
        }


        public static void ShowFloatMenuSkyCloudCores(Action<Building> onClick, Building self = null)
        {
            List<FloatMenuOption> opts = new List<FloatMenuOption>();
            FloatMenu floatMenuMap;

            //Listing des SkyCloud Cores
            foreach (var core in Utils.GCATPP.getAvailableSkyCloudCores())
            {
                if (core == self)
                    continue;
                //SI colon vivant et relié au RimNet et pas dans la liste d'exception et possede une PUCE RIMNET
                if (!core.Destroyed && core.TryGetComp<CompPowerTrader>().PowerOn)
                {
                    CompSkyCloudCore ccore = core.TryGetComp<CompSkyCloudCore>();
                    if (ccore == null)
                        continue;

                    opts.Add(new FloatMenuOption(ccore.getName(), delegate
                    {
                        onClick(core);
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
            }
            opts.SortBy((x) => x.Label);

            //SI pas choix affichage de la raison 
            if (opts.Count == 0)
                opts.Add(new FloatMenuOption("ATPP_NoSkyCloudCoreAvailable".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null));

            floatMenuMap = new FloatMenu(opts, "");
            Find.WindowStack.Add(floatMenuMap);
        }

        public static void Duplicate(Pawn source, Pawn dest, bool overwriteAsDeath=true)
        {
            try
            {
                string destOrigLabelShort = dest.LabelShortCap;

                //Log.Message("Source => " + source.Label + " Dest => " + dest.Label);
                //********************* Duplication de la story
                Pawn_StoryTracker st = new Pawn_StoryTracker(dest);
                //Recopie atraits physique de la destination
                st.melanin = dest.story.melanin;
                UnityEngine.Color hair = new UnityEngine.Color();
                hair.a = dest.story.hairColor.a;
                hair.r = dest.story.hairColor.r;
                hair.g = dest.story.hairColor.g;
                hair.b = dest.story.hairColor.b;
                st.hairColor = hair;
                st.crownType = dest.story.crownType;
                st.hairDef = dest.story.hairDef;
                //duplication adultHood de la source
                if (source.story != null && source.story.adulthood != null)
                {
                    Backstory ah = new Backstory();
                    BackstoryDatabase.TryGetWithIdentifier(source.story.adulthood.identifier, out ah);
                    st.adulthood = ah;
                }
                //duplication childHood de la source
                Backstory ch = new Backstory();
                BackstoryDatabase.TryGetWithIdentifier(source.story.childhood.identifier, out ch);
                st.childhood = ch;
                //Recopie attraits du corp de la destination
                st.bodyType = dest.story.bodyType;
                //duplication des traits de la source 
                foreach (var trait in source.story.traits.allTraits)
                {
                    Trait nt = new Trait(trait.def, trait.Degree, false);
                    //st1.traits.GainTrait(nt);
                    gainDirectTrait(st, nt);
                }
                st.title = dest.story.title;

                string vhg1 = (string)Traverse.Create(dest.story).Field("headGraphicPath").GetValue();

                dest.story = st;

                Traverse.Create(dest.story).Field("headGraphicPath").SetValue(vhg1);

                notifTraitsChanged(dest);

                Pawn_SkillTracker ps = new Pawn_SkillTracker(source);
                //****************************  Duplication skills de la source
                List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
                for (int i = 0; i < allDefsListForReading.Count; i++)
                {
                    SkillDef skillDef = allDefsListForReading[i];
                    SkillRecord skill = ps.GetSkill(skillDef);
                    SkillRecord skillSource = source.skills.GetSkill(skillDef);
                    skill.Level = skillSource.Level;

                    if (!skillSource.TotallyDisabled)
                    {
                        skill.passion = skillSource.passion;
                        skill.xpSinceLastLevel = skillSource.xpSinceLastLevel;
                        skill.xpSinceMidnight = skillSource.xpSinceMidnight;
                    }
                }
                dest.skills = ps;

                //Simulation mort player
                if (overwriteAsDeath)
                {
                    PawnDiedOrDownedThoughtsUtility.TryGiveThoughts(dest, null, PawnDiedOrDownedThoughtsKind.Died);

                    Pawn spouse = dest.GetSpouse();
                    if (spouse != null && !spouse.Dead && spouse.needs.mood != null)
                    {
                        MemoryThoughtHandler memories = spouse.needs.mood.thoughts.memories;
                        memories.RemoveMemoriesOfDef(ThoughtDefOf.GotMarried);
                        memories.RemoveMemoriesOfDef(ThoughtDefOf.HoneymoonPhase);
                    }
                    Traverse.Create(dest.relations).Method("AffectBondedAnimalsOnMyDeath").GetValue();
                }



                dest.relations = new Pawn_RelationsTracker(dest);

                Pawn_NeedsTracker pn = new Pawn_NeedsTracker(dest);

                //Rajout des memoires personalisées
                foreach (var x in source.needs.mood.thoughts.memories.Memories)
                {
                    if (x.otherPawn != null && x.otherPawn == source)
                        x.otherPawn = dest;

                    pn.mood.thoughts.memories.Memories.Add(x);
                }

                dest.needs = pn;

                //Affichage message de mort fake du destinataire
                if (overwriteAsDeath)
                    dest.health.NotifyPlayerOfKilled(null, null, null);

                NameTriple nam = (NameTriple)source.Name;
                dest.Name = new NameTriple(nam.First, nam.Nick, nam.Last);

                dest.Drawer.renderer.graphics.ResolveAllGraphics();

                //Ajout malus de la mort du destinataire
            }
            catch(Exception e)
            {
                Log.Message("[ATPP] Utils.Duplicate : " + e.Message + " - " + e.StackTrace);
            }
        }

        public static void PermutePawn(Pawn p1, Pawn p2)
        {
            try
            {
                if (p1 == null || p2 == null)
                    return;

                //Duplication story 1
                Pawn_StoryTracker st1 = new Pawn_StoryTracker(p2);
                //Recopie atraits physique de la destination
                st1.melanin = p2.story.melanin;
                
                UnityEngine.Color hair1 = new UnityEngine.Color();
                hair1.a = p2.story.hairColor.a;
                hair1.r = p2.story.hairColor.r;
                hair1.g = p2.story.hairColor.g;
                hair1.b = p2.story.hairColor.b;
                //Log.Message("L1");
                st1.hairColor = hair1;
                st1.crownType = p2.story.crownType;
                st1.hairDef = p2.story.hairDef;
                //duplication adultHood de la source
                Backstory ah = new Backstory();
                if (p1.story != null && p1.story.adulthood != null)
                {
                    BackstoryDatabase.TryGetWithIdentifier(p1.story.adulthood.identifier, out ah);
                    st1.adulthood = ah;
                    //duplication childHood de la source
                }
                Backstory ch = new Backstory();
                BackstoryDatabase.TryGetWithIdentifier(p1.story.childhood.identifier, out ch);
                st1.childhood = ch;
                //Log.Message("L2");
                //Recopie attraits du corp de la destination
                if (p2.story != null && p2.story.bodyType != null)
                    st1.bodyType = p2.story.bodyType;

                //duplication des traits de la source 
                foreach (var trait in p1.story.traits.allTraits)
                {
                    Trait nt = new Trait(trait.def, trait.Degree, false);
                    //st1.traits.GainTrait(nt);
                    gainDirectTrait(st1, nt);
                }
                st1.title = p2.story.title;

                //Log.Message("L3");
                //Duplication story 2
                Pawn_StoryTracker st2 = new Pawn_StoryTracker(p1);
                //Recopie atraits physique de la destination
                st2.melanin = p1.story.melanin;
                UnityEngine.Color hair2 = new UnityEngine.Color();
                hair2.a = p1.story.hairColor.a;
                hair2.r = p1.story.hairColor.r;
                hair2.g = p1.story.hairColor.g;
                hair2.b = p1.story.hairColor.b;
                st2.hairColor = hair2;
                //Log.Message("L4");
                st2.crownType = p1.story.crownType;
                st2.hairDef = p1.story.hairDef;
                //duplication adultHood de la source
                if (p2.story != null && p2.story.adulthood != null)
                {
                    Backstory ah2 = new Backstory();
                    BackstoryDatabase.TryGetWithIdentifier(p2.story.adulthood.identifier, out ah2);
                    st2.adulthood = ah2;
                }
                //duplication childHood de la source
                Backstory ch2 = new Backstory();
                BackstoryDatabase.TryGetWithIdentifier(p2.story.childhood.identifier, out ch2);
                st2.childhood = ch2;
                //Log.Message("L5");
                //Recopie attraits du corp de la destination
                st2.bodyType = p1.story.bodyType;
                //duplication des traits de la source 
                foreach (var trait in p2.story.traits.allTraits)
                {
                    Trait nt = new Trait(trait.def, trait.Degree, false);
                    //st2.traits.GainTrait(nt);
                    gainDirectTrait(st2, nt);
                }
                st2.title = p1.story.title;


                string vhg1 = (string) Traverse.Create(p1.story).Field("headGraphicPath").GetValue();
                string vhg2 = (string)Traverse.Create(p2.story).Field("headGraphicPath").GetValue();


                p1.story = st2;
                p2.story = st1;

                Traverse.Create(p1.story).Field("headGraphicPath").SetValue(vhg1);
                Traverse.Create(p2.story).Field("headGraphicPath").SetValue(vhg2);
                //Log.Message("L6");


                notifTraitsChanged(p1);
                notifTraitsChanged(p2);

                ResetCachedIncapableOf(p1);
                ResetCachedIncapableOf(p2);

                invertRelations(p1, p2);

                //Repercution des changements sur les worldPawns
                /*foreach (var wp in Find.WorldPawns.AllPawnsAlive)
                {
                    foreach(var rel in wp.relations.DirectRelations.ToList())
                    {
                        if (rel.otherPawn != null)
                        {
                            if (rel.otherPawn == p1)
                            {
                                rel.otherPawn = p2;
                            }
                            else if (rel.otherPawn == p2)
                            {
                                rel.otherPawn = p1;
                            }
                        }
                    }
                }*/
                //Repercution des changements sur les colons

                //Log.Message("L10");
                /************ SKILLS ****************/

                Pawn_SkillTracker ps1 = new Pawn_SkillTracker(p1);
                Pawn_SkillTracker ps2 = new Pawn_SkillTracker(p2);

                List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
                for (int i = 0; i < allDefsListForReading.Count; i++)
                {
                    SkillDef skillDef = allDefsListForReading[i];
                    SkillRecord skill = ps2.GetSkill(skillDef);
                    SkillRecord skillSource = p1.skills.GetSkill(skillDef);
                    skill.levelInt = skillSource.levelInt;

                    //if (!skillSource.TotallyDisabled)
                    //{
                        skill.passion = skillSource.passion;
                        skill.xpSinceLastLevel = skillSource.xpSinceLastLevel;
                        skill.xpSinceMidnight = skillSource.xpSinceMidnight;
                    //}

                    SkillDef skillDef2 = allDefsListForReading[i];
                    SkillRecord skill2 = ps1.GetSkill(skillDef);
                    SkillRecord skillSource2 = p2.skills.GetSkill(skillDef);
                    skill2.levelInt = skillSource2.levelInt;

                    //if (!skillSource2.TotallyDisabled)
                    //{
                        skill2.passion = skillSource2.passion;
                        skill2.xpSinceLastLevel = skillSource2.xpSinceLastLevel;
                        skill2.xpSinceMidnight = skillSource2.xpSinceMidnight;
                    //}
                }

                p1.skills = ps1;
                p2.skills = ps2;

                Pawn_TrainingTracker ptt = p1.training;
                p1.training = p2.training;
                p2.training = ptt;


                /*Pawn_MindState pmt = p1.mindState;
                p1.mindState = p2.mindState;
                p2.mindState = pmt;*/



                /****************** Changement du journal ************************************/

                foreach (var log in Find.PlayLog.AllEntries)
                {
                    if (log.Concerns(p1) || log.Concerns(p2))
                    {
                        Traverse tlog = Traverse.Create(log);
                        Pawn initiator = tlog.Field("initiator").GetValue<Pawn>();
                        Pawn recipient = tlog.Field("recipient").GetValue<Pawn>();

                        if (initiator == p1)
                            initiator = p2;
                        else if (initiator == p2)
                            initiator = p1;

                        if (recipient == p2)
                            recipient = p2;
                        else if (recipient == p1)
                            recipient = p2;

                        tlog.Field("initiator").SetValue(initiator);
                        tlog.Field("recipient").SetValue(recipient);

                        //Log.Message("IIC");
                    }
                }


                /****************** memoires ****************************/

                //Recreer les moods et les attribués de maniere croisée
                Pawn_NeedsTracker pn1 = new Pawn_NeedsTracker(p1);
                Pawn_NeedsTracker pn2 = new Pawn_NeedsTracker(p2);

                //Rajout des memoires personalisées
                foreach (var x in p1.needs.mood.thoughts.memories.Memories)
                {
                    if (x.otherPawn != null && x.otherPawn == p2)
                        x.otherPawn = p1;

                    pn2.mood.thoughts.memories.Memories.Add(x);
                }

                foreach (var x in p2.needs.mood.thoughts.memories.Memories)
                {
                    if (x.otherPawn != null && x.otherPawn == p1)
                        x.otherPawn = p2;

                    pn1.mood.thoughts.memories.Memories.Add(x);
                }

                //Changement des memories de l'ensemble des pawns si une memoire avec les protagonistes locaux et wp

                foreach (var map in Find.Maps)
                {
                    foreach (var colon in map.mapPawns.AllPawns)
                    {
                        if (colon == p1 || colon == p2 || colon.needs.mood == null)
                            continue;

                        foreach (var mem in colon.needs.mood.thoughts.memories.Memories)
                        {
                            if (mem.otherPawn != null)
                            {
                                if (mem.otherPawn == p1)
                                {
                                    mem.otherPawn = p2;
                                }
                                else if (mem.otherPawn == p2)
                                {
                                    mem.otherPawn = p1;
                                }
                            }
                        }
                    }
                }

                foreach (var wp in Find.WorldPawns.AllPawnsAlive)
                {
                    if (wp.needs.mood == null)
                        continue;

                    foreach (var mem in wp.needs.mood.thoughts.memories.Memories)
                    {
                        if (mem.otherPawn != null)
                        {
                            if (mem.otherPawn == p1)
                            {
                                mem.otherPawn = p2;
                            }
                            else if (mem.otherPawn == p2)
                            {
                                mem.otherPawn = p1;
                            }
                        }
                    }
                }



                /******************* NEEDS *******************************/




                //Log.Message("KJJJ");
                //Report des indicateurs des corps
                if (p1.needs.food != null && p2.needs.food != null)
                {
                    pn1.food.CurLevel = p1.needs.food.CurLevel;
                    pn2.food.CurLevel = p2.needs.food.CurLevel;
                }
                //Log.Message("KJJJ2");
                if (p1.needs.joy != null && p2.needs.joy != null)
                {
                    pn1.joy.CurLevel = p1.needs.joy.CurLevel;
                    pn2.joy.CurLevel = p2.needs.joy.CurLevel;
                }
                //Log.Message("KJJJ3");
                if (p1.needs.comfort != null && p2.needs.comfort != null)
                {
                    pn1.comfort.CurLevel = p1.needs.comfort.CurLevel;
                    pn2.comfort.CurLevel = p2.needs.comfort.CurLevel;
                }
                //Log.Message("KJJJ4");

                if (p1.needs.roomsize != null && p2.needs.roomsize != null)
                {
                    pn1.roomsize.CurLevel = p1.needs.roomsize.CurLevel;
                    pn2.roomsize.CurLevel = p2.needs.roomsize.CurLevel;
                }
                //Log.Message("KJJJ5");

                if (p1.needs.rest != null && p2.needs.rest != null)
                {
                    pn1.rest.CurLevel = p1.needs.rest.CurLevel;
                    pn2.rest.CurLevel = p2.needs.rest.CurLevel;
                }

                if (p1.needs.mood != null && p2.needs.mood != null)
                {
                    pn1.mood.CurLevel = p2.needs.mood.CurLevel;
                    pn2.mood.CurLevel = p1.needs.mood.CurLevel;
                }

                p1.needs = pn1;
                p2.needs = pn2;
                //Log.Message("KJJJ6");
                //Ajout des needs de base relatifs au corp et a la personnalité
                pn1.AddOrRemoveNeedsAsAppropriate();
                pn2.AddOrRemoveNeedsAsAppropriate();

                //Log.Message("KJJJ7");
                /*foreach(var mem in p1.needs.mood.thoughts.memories.Memories)
                {
                    if (mem.otherPawn != null && mem.otherPawn == p1)
                    {
                        mem.otherPawn = p2;
                    }
                }

                foreach (var mem in p2.needs.mood.thoughts.memories.Memories)
                {
                    if (mem.otherPawn != null && mem.otherPawn == p2)
                    {
                        mem.otherPawn = p1;
                    }
                }*/

                /*ThingDef p1TD = p1.def;
                p1.def = p2.def;
                p2.def = p1TD;*/
                //Log.Message("KJJJ8");
                Name nam = p1.Name;
                p1.Name = p2.Name;
                p2.Name = nam;

                p1.Drawer.renderer.graphics.ResolveAllGraphics();
                p2.Drawer.renderer.graphics.ResolveAllGraphics();

                
                /*************************************** PERMUTATION des LITS ********************************/
                Building_Bed bedP1 = p1.ownership.OwnedBed;
                Building_Bed bedP2 = p2.ownership.OwnedBed;

                /*if (bedP1 != null && bedP2 != null)
                {
                    p1.ownership.UnclaimBed();
                    p2.ownership.UnclaimBed();

                    p1.ownership.ClaimBedIfNonMedical(bedP2);
                    p2.ownership.ClaimBedIfNonMedical(bedP1);
                }*/
                if(bedP1 != null)
                    p1.ownership.UnclaimBed();
                if(bedP2 != null)
                    p2.ownership.UnclaimBed();

                if (bedP1 != null)
                {
                    p2.ownership.ClaimBedIfNonMedical(bedP1);
                }
                if(bedP2 != null)
                {
                    p1.ownership.ClaimBedIfNonMedical(bedP2);
                }
                
                //Log.Message("KJJJ9");
                /*Log.Message("P2 => " + p2.Label);
                foreach (Pawn current in p2.relations.pawnsWithDirectRelationsWithMe)
                {
                    Log.Message("=>" + current.Label);
                }*/
            }
            catch(Exception e)
            {
                Log.Message("[ATPP] Utils.PermutePawn : " + e.Message + " - " + e.StackTrace);
            }
        }

        public  static bool ShouldBeDead(this Pawn pawn)
        {
            if (pawn.Dead)
            {
                return true;
            }
            for (int i = 0; i < pawn.health.hediffSet.hediffs.Count; i++)
            {
                if (pawn.health.hediffSet.hediffs[i].CauseDeathNow())
                {
                    return true;
                }
            }
            if (pawn.health.ShouldBeDeadFromRequiredCapacity() != null)
            {
                return true;
            }
            float num = PawnCapacityUtility.CalculatePartEfficiency(pawn.health.hediffSet, pawn.RaceProps.body.corePart, false, null);
            return num <= 0.0001f || pawn.health.ShouldBeDeadFromLethalDamageThreshold();
        }


        public static void ResetCachedIncapableOf(Pawn pawn)
        {
            pawn.ClearCachedDisabledWorkTypes();
            pawn.ClearCachedDisabledSkillRecords();
            List<string> incapableList = new List<string>();
            WorkTags combinedDisabledWorkTags = pawn.CombinedDisabledWorkTags;
            if (combinedDisabledWorkTags != WorkTags.None)
            {
                IEnumerable<WorkTags> list = (IEnumerable<WorkTags>)typeof(CharacterCardUtility).GetMethod("WorkTagsFrom", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { combinedDisabledWorkTags });
                foreach (var tag in list)
                {
                    incapableList.Add(WorkTypeDefsUtility.LabelTranslated(tag).CapitalizeFirst());
                }
            }
        }

        public static void ClearCachedDisabledWorkTypes(this Pawn pawn)
        {
            if (pawn != null)
            {
                typeof(Pawn).GetField("cachedDisabledWorkTypes", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(pawn, null);
            }
        }

        public static void ClearCachedDisabledSkillRecords(this Pawn pawn)
        {
            if (pawn.skills != null && pawn.skills.skills != null)
            {
                FieldInfo field = typeof(SkillRecord).GetField("cachedTotallyDisabled", BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var record in pawn.skills.skills)
                {
                    field.SetValue(record, BoolUnknown.Unknown);
                }
            }
        }

        public static void invertRelations(Pawn p1,Pawn p2)
        {
            try
            {
                Pawn_RelationsTracker pro1 = p1.relations;
                Pawn_RelationsTracker pro2 = p2.relations;

                Pawn_RelationsTracker pr1 = new Pawn_RelationsTracker(p1);
                Pawn_RelationsTracker pr2 = new Pawn_RelationsTracker(p2);
                p1.relations = pr1;
                p2.relations = pr2;
                Pawn tmp1;
                //Log.Message("L7");
                //Suppression relations des worldPawns avec p1 et p2
                foreach (var wp in Find.WorldPawns.AllPawnsAlive)
                {
                    if (wp == null || wp.relations == null || wp.relations.DirectRelations == null)
                        continue;

                    foreach (var rel in wp.relations.DirectRelations.ToList())
                    {
                        if (rel.otherPawn != null)
                        {
                            if (rel.otherPawn == p1)
                            {
                                //rel.otherPawn = p2;
                                wp.relations.TryRemoveDirectRelation(rel.def, p1);
                                wp.relations.AddDirectRelation(rel.def, p2);
                            }
                            if (rel.otherPawn == p2)
                            {
                                //rel.otherPawn = p1;
                                wp.relations.TryRemoveDirectRelation(rel.def, p2);
                                wp.relations.AddDirectRelation(rel.def, p1);
                            }
                        }
                    }
                }
                //Log.Message("L8");
                //Constitution nouvelle liste des relations de P1
                foreach (var rel in pro2.DirectRelations.ToList())
                {
                    if (rel.otherPawn != null && rel.otherPawn.relations != null && rel.otherPawn != p1 && rel.otherPawn != p2)
                    {
                        rel.otherPawn.relations.TryRemoveDirectRelation(rel.def, p2);
                    }
                    //Log.Message(rel.def.defName + " " + rel.otherPawn.Label);

                    if (rel.otherPawn == p1)
                        tmp1 = p2;
                    else
                        tmp1 = rel.otherPawn;

                    pr1.AddDirectRelation(rel.def, tmp1);
                }
                //Log.Message("L9");
                //Consitution nouvelle liste des relations de P2
                foreach (var rel in pro1.DirectRelations.ToList())
                {
                    if (rel.otherPawn != null && rel.otherPawn.relations != null  && rel.otherPawn != p1 && rel.otherPawn != p2)
                    {
                        rel.otherPawn.relations.TryRemoveDirectRelation(rel.def, p1);
                    }
                    //Log.Message(rel.def.defName + " " + rel.otherPawn.Label);

                    if (rel.otherPawn == p2)
                        tmp1 = p1;
                    else
                        tmp1 = rel.otherPawn;

                    pr2.AddDirectRelation(rel.def, tmp1);
                }

                p1.relations.everSeenByPlayer = true;
                p2.relations.everSeenByPlayer = true;

                foreach (var map in Find.Maps)
                {
                    foreach (var colon in map.mapPawns.AllPawns)
                    {
                        if (colon == p1 || colon == p2)
                            continue;

                        /*Log.Message("Colon : " + colon.LabelCap);
                        foreach (var rel in colon.relations.DirectRelations.ToList())
                        {
                            Log.Message(rel.def.defName);
                            if (rel.otherPawn != null && !RelationsException.Contains(rel.def.defName)) //rel.def.defName != "Bond" && rel.def.defName != "Parent")
                            {
                                if (rel.otherPawn == p1)
                                {
                                    rel.otherPawn = p2;
                                }
                                else if (rel.otherPawn == p2)
                                {
                                    rel.otherPawn = p1;
                                }
                            }
                        }*/

                        //Maitre d'un animal est un des permutés ? => transfert a l'autre
                        if (colon.playerSettings != null)
                        {
                            if (colon.playerSettings.Master != null && colon.playerSettings.Master == p1)
                                colon.playerSettings.Master = p2;
                            else if (colon.playerSettings.Master != null && colon.playerSettings.Master == p2)
                                colon.playerSettings.Master = p1;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.Message("[ATPP] Utils.InvertRelations : " + e.Message + " - " + e.StackTrace);
            }
        }


        public static void gainDirectTrait(Pawn_StoryTracker tr,  Trait trait)
        {
            if (tr.traits.HasTrait(trait.def))
                return;

            tr.traits.allTraits.Add(trait);
        }

        public static void notifTraitsChanged(Pawn pawn)
        {
            pawn.Notify_DisabledWorkTypesChanged();

            //Traverse.Create(pawn.story).Method("Notify_TraitChanged").GetValue();

            if (pawn.skills != null)
            {
                pawn.skills.Notify_SkillDisablesChanged();
            }
            if (!pawn.Dead && pawn.RaceProps.Humanlike)
            {
                pawn.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();
            }
        }

        public static bool pawnCurrentlyControlRemoteSurrogate(Pawn pawn)
        {
            CompSurrogateOwner cso = pawn.TryGetComp<CompSurrogateOwner>();
            return (cso != null && cso.isThereSX());
        }


        public static void removeUploadHediff(Pawn cpawn, Pawn uploadRecipient)
        {
            Hediff he;

            //On eleve le hediff
            if (cpawn != null)
            {
                he = cpawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("ATPP_ConsciousnessUpload"));
                if (he != null)
                    cpawn.health.RemoveHediff(he);
            }

            if (uploadRecipient != null)
            {
                he = uploadRecipient.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("ATPP_ConsciousnessUpload"));
                if (he != null)
                    uploadRecipient.health.RemoveHediff(he);
            }
        }


        public static bool mindTransfertsAllowed(Pawn pawn, bool checkIsBlankAndroid=true)
        {
            CompAndroidState cas = pawn.TryGetComp<CompAndroidState>();
            if (cas != null && (cas.uploadEndingGT != -1 || cas.showUploadProgress || (checkIsBlankAndroid && cas.isBlankAndroid)))
                return false;

            CompSurrogateOwner cso = pawn.TryGetComp<CompSurrogateOwner>();
            if (cso != null)
            {
                if (cso.duplicateEndingGT != -1 || cso.showDuplicateProgress)
                    return false;
                if (cso.permuteEndingGT != -1 || cso.showPermuteProgress)
                    return false;
                if (cso.uploadToSkyCloudEndingGT != -1)
                    return false;
                if (cso.downloadFromSkyCloudEndingGT != -1)
                    return false;
                if (cso.mindAbsorptionEndingGT != -1)
                    return false;
            }

            return true && !pawn.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare); ;
        }


        public static bool isThereNotControlledSurrogateInCaravan()
        {
            foreach(var c in Find.World.worldObjects.Caravans)
            {
                foreach(var p in c.pawns)
                {
                    if (!p.Dead
                        && !p.Destroyed && p.IsSurrogateAndroid())
                    {
                        CompAndroidState cas = p.TryGetComp<CompAndroidState>();
                        if (cas.surrogateController == null)
                            return true;
                    }
                }
            }

            return false;
        }


        public static void ShowFloatMenuNotCOntrolledSurrogateInCaravan(Pawn emitter, Action<Pawn> onClick)
        {
            List<FloatMenuOption> opts = new List<FloatMenuOption>();
            FloatMenu floatMenuMap;

            foreach(var c in Find.World.worldObjects.Caravans){
                //Listing des colons sauf ceux présents dans la liste d'exception (exp)
                foreach (var colon in c.pawns)
                {
                    //SI colon vivant et relié au RimNet et pas dans la liste d'exception et possede une PUCE RIMNET
                    if (colon != emitter && !colon.Dead
                        && !colon.Destroyed && colon.IsSurrogateAndroid(false, true))
                    {
                        opts.Add(new FloatMenuOption(colon.LabelShortCap, delegate
                        {
                            onClick(colon);
                        }, MenuOptionPriority.Default, null, null, 0f, null, null));
                    }
                }
            }
            opts.SortBy((x) => x.Label);

            //SI pas choix affichage de la raison 
            if (opts.Count == 0)
                return;

            floatMenuMap = new FloatMenu(opts, "");
            Find.WindowStack.Add(floatMenuMap);
        }


        public static Pawn spawnCorpseCopy(Pawn pawn, bool kill=true)
        {
            /*PawnGenerationContext pgc = PawnGenerationContext.NonPlayer;
            if (!kill)
                pgc = PawnGenerationContext.PlayerStarter;*/

            PawnGenerationRequest request = new PawnGenerationRequest(kind: pawn.kindDef, faction: Faction.OfAncients, context: PawnGenerationContext.NonPlayer, fixedBiologicalAge: pawn.ageTracker.AgeBiologicalYearsFloat, fixedChronologicalAge: pawn.ageTracker.AgeChronologicalYearsFloat, fixedGender: pawn.gender, fixedMelanin: pawn.story.melanin);
            Pawn p = PawnGenerator.GeneratePawn(request);


            p?.equipment?.DestroyAllEquipment();
            p?.apparel?.DestroyAll();
            p?.inventory?.DestroyAll();

            p.Rotation = pawn.Rotation;
            //duplication apparence physique
            //p.gender = pawn.gender;
            p.story.melanin = pawn.story.melanin;
            p.story.bodyType = pawn.story.bodyType;
            UnityEngine.Color hair = new UnityEngine.Color();
            hair.a = pawn.story.hairColor.a;
            hair.r = pawn.story.hairColor.r;
            hair.g = pawn.story.hairColor.g;
            hair.b = pawn.story.hairColor.b;
            p.story.hairColor = hair;
            p.story.crownType = pawn.story.crownType;
            p.story.hairDef = pawn.story.hairDef;


            if (pawn.inventory != null && pawn.inventory.innerContainer != null && p.inventory != null && p.inventory.innerContainer != null)
            {
                try
                {
                    pawn.inventory.innerContainer.TryTransferAllToContainer(p.inventory.innerContainer);
                }
                catch (Exception ex)
                {
                    Log.Message("[ATPP] Utils.SpawnCorpse.transfertInventory " + ex.Message + " " + ex.StackTrace);
                }
            }

            if (pawn.equipment != null && p.equipment != null)
            {
                foreach (var e in pawn.equipment.AllEquipmentListForReading.ToList())
                {
                    try
                    {
                        pawn.equipment.Remove(e);
                        p.equipment.AddEquipment(e);
                    }
                    catch (Exception ex)
                    {
                        Log.Message("[ATPP] Utils.SpawnCorpse.transfertEquipment " + ex.Message + " " + ex.StackTrace);
                    }
                }
            }

            //Transfert vetements
            if (pawn.apparel != null)
            {
                p.apparel.DestroyAll();

                //Log.Message("--Traitement des vetements");
                foreach (var e in pawn.apparel.WornApparel.ToList())
                {
                    pawn.apparel.Remove(e);
                    p.apparel.Wear(e);
                }
            }


            p.health.hediffSet.Clear();

            //Ajout des hediffs
            foreach (var h in pawn.health.hediffSet.hediffs)
            {
                try
                {
                    h.pawn = p;
                    p.health.AddHediff(h, h.Part);
                }
                catch(Exception)
                {

                }
            }

            GenSpawn.Spawn(p, pawn.Position, pawn.Map);

            //Duplication mental de la source
            Duplicate(pawn, p, false);
            //Traverse.Create(p.story).Field("headGraphicPath").SetValue(pawn.story.HeadGraphicPath);

            //Report crownType stocké dans le comp d'alien race
            ThingComp alienComp = TryGetCompByTypeName(pawn, "AlienComp", "AlienRace");
            if (alienComp != null)
            {
                string crownType = (string) Traverse.Create(alienComp).Field("crownType").GetValue();

                ThingComp alienComp2 = TryGetCompByTypeName(p, "AlienComp", "AlienRace");
                if(alienComp2 != null)
                {
                    Traverse.Create(alienComp2).Field("crownType").SetValue(crownType);
                }
            }

            //headGraphicPathToUse = pawn.story.HeadGraphicPath;
            p.Drawer.renderer.graphics.ResolveAllGraphics();
            //headGraphicPathToUse = "";

            //p.Drawer.renderer.graphics.headGraphic = GraphicDatabaseHeadRecords.GetHeadNamed(pawn.story.HeadGraphicPath, p.story.SkinColor);
            //Traverse.Create(p.story).Field("headGraphicPath").SetValue(GraphicDatabaseHeadRecords.GetHeadNamed(pawn.story.HeadGraphicPath, p.story.SkinColor).GraphicPath);

            if (kill)
            {
                p.Kill(null, null);
                p.SetFactionDirect(Faction.OfPlayer);
            }
            else
            {
                p.relations.everSeenByPlayer = true;

                p.SetFaction(Faction.OfPlayer);

                //Tentative de connexion au SkyMind
                Utils.GCATPP.connectUser(p);
            }

            return p;
        }

        public static int nbSecuritySlotsGeneratedBy(Building build)
        {
            if (build.def.defName == Utils.defNameOldSecurityServer)
                return Settings.securitySlotForOldSecurityServers;
            else if (build.def.defName == Utils.defNameBasicSecurityServer)
                return Settings.securitySlotForBasicSecurityServers;
            else if (build.def.defName == Utils.defNameAdvancedSecurityServer)
                return Settings.securitySlotForAdvancedSecurityServers;
            else
                return 0;
        }

        public static int nbHackingSlotsGeneratedBy(Building build)
        {
            if (build.def.defName == Utils.defNameOldHackingServer)
                return Settings.hackingSlotsForOldHackingServers;
            else if (build.def.defName == Utils.defNameBasicHackingServer)
                return Settings.hackingSlotsForBasicHackingServers;
            else if (build.def.defName == Utils.defNameAdvancedHackingServer)
                return Settings.hackingSlotsForAdvancedHackingServers;
            else
                return 0;
        }

        public static int nbSkillSlotsGeneratedBy(Building build)
        {
            if (build.def.defName == Utils.defNameOldSkillServer)
                return Settings.skillSlotsForOldSkillServers;
            else if (build.def.defName == Utils.defNameBasicSkillServer)
                return Settings.skillSlotsForBasicSkillServers;
            else if (build.def.defName == Utils.defNameAdvancedSkillServer)
                return Settings.skillSlotsForAdvancedSkillServers;
            else
                return 0;
        }

        public static int nbHackingPointsGeneratedBy(Building build)
        {
            if (build.def.defName == Utils.defNameOldHackingServer)
                return Settings.hackingNbpGeneratedOld;
            else if (build.def.defName == Utils.defNameBasicHackingServer)
                return Settings.hackingNbpGeneratedBasic;
            else if (build.def.defName == Utils.defNameAdvancedHackingServer)
                return Settings.hackingNbpGeneratedAdvanced;
            else
                return 0;
        }

        public static int nbSkillPointsGeneratedBy(Building build)
        {
            if (build.def.defName == Utils.defNameOldSkillServer)
                return Settings.skillNbpGeneratedOld;
            else if (build.def.defName == Utils.defNameBasicSkillServer)
                return Settings.skillNbpGeneratedBasic;
            else if (build.def.defName == Utils.defNameAdvancedSkillServer)
                return Settings.skillNbpGeneratedAdvanced;
            else
                return 0;
        }

        public static int getNbSkillPointsPerSkill(Pawn pawn, bool isMind=false)
        {
            if (isMind)
                return Settings.nbSkillPointsPerSkillT3;
            else if (pawn.def.defName == Utils.T1)
                return Settings.nbSkillPointsPerSkillT1;
            else if (pawn.def.defName == Utils.T2 || pawn.def.defName == Utils.TX2 || pawn.def.defName == Utils.TX2I || pawn.def.defName == Utils.TX2K)
                return Settings.nbSkillPointsPerSkillT2;
            else if (pawn.def.defName == Utils.T3 || pawn.def.defName == Utils.TX3 || pawn.def.defName == Utils.TX3I)
                return Settings.nbSkillPointsPerSkillT3;
            else if (pawn.def.defName == Utils.T4 || pawn.def.defName == Utils.TX4 || pawn.def.defName == Utils.TX3I)
                return Settings.nbSkillPointsPerSkillT4;
            else if (pawn.def.defName == Utils.T5)
                return Settings.nbSkillPointsPerSkillT5;

            return 0;
        }

        public static int getNbSkillPointsToIncreasePassion(Pawn pawn, bool isMind = false)
        {
            if (isMind)
                return Settings.nbSkillPointsPassionT3;
            else if (pawn.def.defName == Utils.T1)
                return Settings.nbSkillPointsPassionT1;
            else if (pawn.def.defName == Utils.T2 || pawn.def.defName == Utils.TX2 || pawn.def.defName == Utils.TX2I || pawn.def.defName == Utils.TX2K)
                return Settings.nbSkillPointsPassionT2;
            else if (pawn.def.defName == Utils.T3 || pawn.def.defName == Utils.TX3 || pawn.def.defName == Utils.TX3I)
                return Settings.nbSkillPointsPassionT3;
            else if (pawn.def.defName == Utils.T4 || pawn.def.defName == Utils.TX4 || pawn.def.defName == Utils.TX4I)
                return Settings.nbSkillPointsPassionT4;
            else if (pawn.def.defName == Utils.T5)
                return Settings.nbSkillPointsPassionT5;

            return 0;
        }

        public static ThingComp TryGetCompByTypeName(ThingWithComps thing, string typeName, string assemblyName = "")
        {
            return thing.AllComps.FirstOrDefault((ThingComp comp) => comp.GetType().Name == typeName);
        }

        public static GameComponent TryGetGameCompByTypeName( string typeName)
        {
            return Current.Game.components.FirstOrDefault((GameComponent comp) => comp.GetType().Name == typeName);
        }

        public static void removeAllTraits(Pawn target)
        {
            target.story.traits.allTraits.Clear();

            //Si T1 rajout du trait simpleMinded
            if (target.def.defName == Utils.T1)
            {
                TraitDef td = DefDatabase<TraitDef>.GetNamed("SimpleMindedAndroid", false);
                Trait t = null;
                if (td != null)
                    t = new Trait(td);
                if (t != null)
                    target.story.traits.allTraits.Add(t);
            }

            //Notif changement des traits
            Utils.notifTraitsChanged(target);
        }

        public static void applySolarFlarePolicy()
        {
            HediffDef he = DefDatabase<HediffDef>.GetNamed("ATPP_SolarFlareAndroidImpact", false);
            if (he != null)
            {
                if (Settings.duringSolarFlaresAndroidsShouldBeDowned)
                {
                    he.stages[0].capMods[0].setMax = 0.1f;
                }
                else
                {
                    he.stages[0].capMods[0].setMax = 0.6f;
                }
            }
        }

        public static void applyLivingPlantPolicy()
        {
            foreach (var e in Utils.ExceptionAndroidList)
            {
                if (e == Utils.M7)
                    continue;

                var td = DefDatabase<ThingDef>.GetNamed(e, false);
                if(td != null && td.race != null)
                {
                    if (Settings.androidsCanConsumeLivingPlants)
                    {
                        td.race.foodType = (FoodTypeFlags)3963;
                    }
                    else
                    {
                        td.race.foodType = FoodTypeFlags.OmnivoreHuman;
                    }
                }
            }

        }

        public static void applyT5ClothesPolicy()
        {
            try
            {
                ThingDef td = DefDatabase<ThingDef>.GetNamed("Android5Tier", false);

                if (td == null)
                    return;

                Traverse tr = Traverse.Create(td).Field("alienRace").Field("raceRestriction").Field("onlyUseRaceRestrictedApparel");

                if (Settings.allowT5ToWearClothes)
                {
                    tr.SetValue(false);
                }
                else
                {
                    tr.SetValue(true);
                }

            }
            catch(Exception e)
            {
                Log.Message("[ATPP] Utils.applyT5ClothesPolicy "+e.Message+" "+e.StackTrace);
            }
        }

        public static void removeMindBlacklistedTrait(Pawn mind)
        {
            List<Trait> toDel = null;
            foreach(var t in mind.story.traits.allTraits)
            {
                if (BlacklistMindTraits.Contains(t.def.defName))
                {
                    if (toDel == null)
                        toDel = new List<Trait>();
                    toDel.Add(t);
                }

            }
            if(toDel != null)
            {
                foreach(var t in toDel)
                {
                    mind.story.traits.allTraits.Remove(t);
                }
            }
        }

        public static void removeSimpleMindedTrait(Pawn cpawn)
        {
            if (Settings.removeSimpleMindedTraitOnUpload && cpawn.story.traits.HasTrait(Utils.traitSimpleMinded))
            {
                Trait toDel = null;
                foreach (var t in cpawn.story.traits.allTraits)
                {
                    if (t.def == Utils.traitSimpleMinded)
                    {
                        toDel = t;
                        break;
                    }

                }

                if (toDel != null)
                {
                    cpawn.story.traits.allTraits.Remove(toDel);
                }
            }
        }

        public static void addSimpleMindedTraitForT1(Pawn cpawn)
        {
            if (cpawn.def.defName == Utils.T1 && !cpawn.story.traits.HasTrait(Utils.traitSimpleMinded))
            {
                cpawn.story.traits.GainTrait(new Trait(Utils.traitSimpleMinded, 0, true));
            }
        }

        public static void makeAndroidBatteryOverload(Pawn android)
        {
            float batteryLevel = 1.0f;

            if(android.needs.food != null)
                batteryLevel = android.needs.food.CurLevelPercentage;

            float radius = 0;

            if (android.def.defName == Utils.M7)
            {
                radius =  12 * batteryLevel;
            }
            else
            {
                radius = 5 * batteryLevel;
            }

            if (radius == 0)
                radius = 1;

            GenExplosion.DoExplosion(android.Position, android.Map, radius, DamageDefOf.Bomb, android, -1, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);

            if (!android.Dead)
                android.Kill(null, null);
        }

        public static void clearBlankAndroid(Pawn android)
        {
            if (android.IsBlankAndroid())
            {
                CompAndroidState cas = android.TryGetComp<CompAndroidState>();
                if (cas != null)
                {
                    cas.isBlankAndroid = false;
                    Hediff he = android.health.hediffSet.GetFirstHediffOfDef(Utils.hediffBlankAndroid);
                    if (he != null)
                        android.health.RemoveHediff(he);
                }
            }
        }

        public static bool androidIsValidPodForCharging(Pawn android)
        {
            //Recharge surrogate
            if (android.InBed())
            {
                Building_Bed bed = android.CurrentBed();
                if (bed != null && (Utils.ExceptionSurrogatePod.Contains(bed.def.defName) || Utils.ExceptionSurrogateM7Pod.Contains(bed.def.defName)))
                {
                    CompPowerTrader cpt = bed.TryGetComp<CompPowerTrader>();
                    //Recharge que si alimenté
                    if (!bed.IsBrokenDown() && cpt != null && cpt.PowerOn)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool androidReloadingAtChargingStation(Pawn android)
        {
            if(android.CurJobDef != null && android.CurJobDef.defName == "ATPP_GoReloadBattery")
            {
                foreach (IntVec3 adjPos in android.CellsAdjacent8WayAndInside())
                {
                    List<Thing> thingList = adjPos.GetThingList(android.Map);
                    if (thingList != null)
                    {
                        for (int i = 0; i < thingList.Count; i++)
                        {
                            Building station = thingList[i] as Building;


                            if (station != null && station.Faction == Faction.OfPlayer && station.def.defName == "ATPP_ReloadStation")
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static void changeHARCrownType(Pawn pawn, string type)
        {
            ThingComp alienComp = Utils.TryGetCompByTypeName(pawn, "AlienComp", "AlienRace");
            if (alienComp != null)
            {
                string crownType = (string)Traverse.Create(alienComp).Field("crownType").GetValue();

                ThingComp alienComp2 = Utils.TryGetCompByTypeName(pawn, "AlienComp", "AlienRace");
                if (alienComp2 != null)
                {
                    Traverse.Create(alienComp2).Field("crownType").SetValue(type);
                }
            }
        }

        public static void changeTXBodyType(Pawn cp, int hurtedLevel)
        {
            string type = "";

            if (hurtedLevel ==1)
            {
                if (cp.def.defName == Utils.TX2)
                {
                    if (cp.gender == Gender.Female)
                        type = "ATPP_BodyTypeFemaleHurted12TX";
                    else
                        type = "ATPP_BodyTypeMaleHurted12TX";
                }
                else if (cp.def.defName == Utils.TX2K)
                {
                    if (cp.gender == Gender.Female)
                        type = "ATPP_BodyTypeFemaleHurted12KTX";
                    else
                        type = "ATPP_BodyTypeMaleHurted12KTX";
                }
                else if (cp.def.defName == Utils.TX3)
                {
                    if (cp.gender == Gender.Female)
                        type = "ATPP_BodyTypeFemaleHurted13TX";
                    else
                        type = "ATPP_BodyTypeMaleHurted13TX";
                }
                else if (cp.def.defName == Utils.TX4)
                {
                    if (cp.gender == Gender.Female)
                        type = "ATPP_BodyTypeFemaleHurted14TX";
                    else
                        type = "ATPP_BodyTypeMaleHurted14TX";
                }
            }
            else if (hurtedLevel == 2)
            {
                if (cp.def.defName == Utils.TX2)
                {
                    if (cp.gender == Gender.Female)
                        type = "ATPP_BodyTypeFemaleHurted22TX";
                    else
                        type = "ATPP_BodyTypeMaleHurted22TX";
                }
                else if (cp.def.defName == Utils.TX2K)
                {
                    if (cp.gender == Gender.Female)
                        type = "ATPP_BodyTypeFemaleHurted22KTX";
                    else
                        type = "ATPP_BodyTypeMaleHurted22KTX";
                }
                else if (cp.def.defName == Utils.TX3)
                {
                    if (cp.gender == Gender.Female)
                        type = "ATPP_BodyTypeFemaleHurted23TX";
                    else
                        type = "ATPP_BodyTypeMaleHurted23TX";
                }
                else if (cp.def.defName == Utils.TX4)
                {
                    if (cp.gender == Gender.Female)
                        type = "ATPP_BodyTypeFemaleHurted24TX";
                    else
                        type = "ATPP_BodyTypeMaleHurted24TX";
                }
            }
            else{
                if (cp.gender == Gender.Female)
                    type = "Female";
                else
                    type = "Male";
            }

            BodyTypeDef bd = DefDatabase<BodyTypeDef>.GetNamed(type, false);
            if (bd != null)
                cp.story.bodyType = bd;
        }


        public static Color getHairColor(string color)
        {
            if (color == "gray")
                return new Color(0.65f, 0.65f, 0.65f);
            else if (color == "white")
                return new Color(0.97f, 0.97f, 0.97f);
            else if (color == "blond")
                return new Color(0.8863f, 0.7373f, 0.4549f);
            else if (color == "ginger")
                return new Color(0.9961f, 0.3686f, 0.1412f);
            else if (color == "auburn")
                return new Color(0.6157f, 0.2431f, 0.0471f);
            else
                return new Color(0.15f, 0.15f, 0.15f);
        }

        public static Color getSkinColor(string color)
        {
            if (color == "verylight")
                return new Color(0.90764f, 0.8262f, 0.63333f, 1f);
            else if (color == "light")
                return new Color(0.89764f, 0.75262f, 0.57333f, 1f);
            else if (color == "fair")
                return new Color(0.89803f, 0.701960f, 0.46666f, 1f);
            else if (color == "midbrown")
                return new Color(0.79803f, 0.501960f, 0.36666f, 1f);
            else if (color == "darkbrown")
                return new Color(0.556862f, 0.360784f, 0.219607f, 1f);
            else
                return new Color(0.4176f, 0.2818f, 0.182f, 1f);
        }

        /*
         * Calcul énergie disponible sur le PowerNet spécifié en parametre
         */
        public static float getCurrentAvailableEnergy(PowerNet pn)
        {
            return pn.CurrentStoredEnergy() + ((float)pn.CurrentEnergyGainRate() / CompPower.WattsToWattDaysPerTick);
        }
    }
}
