using UnityEngine;
using Verse;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse.AI;

namespace MOARANDROIDS
{
    public class Settings : ModSettings
    {
        public static bool keepPuppetBackstory = false;
        public static float percentageChanceMaleAndroidModel = 0.5f;
        public static bool allowT5ToWearClothes = false;
        public static int maxAndroidByPortableLWPN = 5;
        public static bool allowAutoRepaint = true;
        public static bool allowAutoRepaintForPrisoners = true;
        public static bool preventM7T5AppearingInCharacterScreen = true;

        public static int nbSkillPointsPassionT1 = 4;
        public static int nbSkillPointsPassionT2 = 5;
        public static int nbSkillPointsPassionT3 = 6;
        public static int nbSkillPointsPassionT4 = 7;
        public static int nbSkillPointsPassionT5 = 8;


        public static bool androidsAreRare = false;
        public static int minDaysAndroidPaintingCanRust = 35;
        public static int maxDaysAndroidPaintingCanRust = 90;

        public static float chanceGeneratedAndroidCanBePaintedOrRust = 0.15f;
        public static bool androidsCanRust = true;
        public static bool removeSimpleMindedTraitOnUpload = true;
        public static int skyCloudUploadModeForSourceMind = 2;
        public static bool allowHumanDrugsForT3PlusAndroids = true;
        public static bool allowHumanDrugsForAndroids = false;

        public static bool removeComfortNeedForT3T4 = true;
        public static int nbSkillPointsPerSkillT1 = 150;
        public static int nbSkillPointsPerSkillT2 = 250;
        public static int nbSkillPointsPerSkillT3 = 600;
        public static int nbSkillPointsPerSkillT4 = 1000;
        public static int nbSkillPointsPerSkillT5 = 1250;

        public static int minHoursNaniteFramework = 8;
        public static int maxHoursNaniteFramework = 48;

        public static bool basicAndroidsRandomSKills = true;
        public static int defaultSkillT1Shoot = 4;
        public static int defaultSkillT1Melee = 4;
        public static int defaultSkillT1Construction = 4;
        public static int defaultSkillT1Mining = 4;
        public static int defaultSkillT1Cooking = 4;
        public static int defaultSkillT1Plants = 4;
        public static int defaultSkillT1Animals = 4;
        public static int defaultSkillT1Crafting = 4;
        public static int defaultSkillT1Artistic = 0;
        public static int defaultSkillT1Medical = 4;
        public static int defaultSkillT1Social = 0;
        public static int defaultSkillT1Intellectual = 0;

        public static int defaultSkillT2Shoot = 6;
        public static int defaultSkillT2Melee = 6;
        public static int defaultSkillT2Construction = 6;
        public static int defaultSkillT2Mining = 6;
        public static int defaultSkillT2Cooking = 6;
        public static int defaultSkillT2Plants = 6;
        public static int defaultSkillT2Animals = 6;
        public static int defaultSkillT2Crafting = 6;
        public static int defaultSkillT2Artistic = 0;
        public static int defaultSkillT2Medical = 6;
        public static int defaultSkillT2Social = 0;
        public static int defaultSkillT2Intellectual = 0;


        public static int VX3MaxSurrogateControllableAtOnce = 6;
        public static int secToBootSkyCloudCore = 30;
        public static bool androidsCanConsumeLivingPlants = false;
        public static bool hideMenuAllowingForceEatingLivingPlants = false;

        public static bool notRemoveAllTraitsFromT1T2 = false;
        public static bool notRemoveAllSkillPassionsForBasicAndroids = false;
        public static bool disableAbilitySkyCloudServerToTalk = false;
        public static int minDurationMentalBreakOfDigitisedMinds = 4;
        public static int maxDurationMentalBreakOfDigitisedMinds = 36;

        public static bool androidsCanOnlyBeHealedByCrafter = false;
        public static bool hideInactiveSurrogates = true;
        public static int nbHourLiteHackingDeviceAttackLastMin = 1;
        public static int nbHourLiteHackingDeviceAttackLastMax = 8;

        public static int hackingSlotsForOldHackingServers = 50;
        public static int hackingSlotsForBasicHackingServers = 100;
        public static int hackingSlotsForAdvancedHackingServers = 200;

        public static int skillSlotsForOldSkillServers = 50;
        public static int skillSlotsForBasicSkillServers = 100;
        public static int skillSlotsForAdvancedSkillServers = 200;

        public static int securitySlotForOldSecurityServers = 2;
        public static int securitySlotForBasicSecurityServers = 5;
        public static int securitySlotForAdvancedSecurityServers = 10;

        public static int skillNbpGeneratedOld = 5;
        public static int skillNbpGeneratedBasic = 10;
        public static int skillNbpGeneratedAdvanced = 20;

        public static int hackingNbpGeneratedOld = 5;
        public static int hackingNbpGeneratedBasic = 10;
        public static int hackingNbpGeneratedAdvanced = 20;

        public static int nbMoodPerAssistingMinds = 1;
        public static bool hideRemotelyControlledDeviceIcon = false;
        public static int powerConsumedByStoredMind = 350;
        public static bool disableLowNetworkMalusInCaravans = true;
        public static bool disableLowNetworkMalus = false;
        public static bool disableServersAlarm = true;
        public static bool disableServersAmbiance = false;
        public static bool otherFactionsCanUseSurrogate = true;
        public static int mindReplicationHours = 2;
        public static int mindUploadHour = 2;
        public static int mindPermutationHours = 4;
        public static int mindDuplicationHours = 8;
        public static int mindUploadToSkyCloudHours = 4;
        public static int mindSkyCloudMigrationHours = 3;

        public static bool duringSolarFlaresAndroidsShouldBeDowned = false;

        public static int defaultGeneratorMode = 1;
        public static float percentageNanitesFail = 0.08f;
        public static int wattConsumedByT1 = 100;
        public static int wattConsumedByT2 = 200;
        public static int wattConsumedByT3 = 300;
        public static int wattConsumedByT4 = 400;
        public static int wattConsumedByT5 = 500;
        public static int wattConsumedByM7 = 700;
        public static int wattConsumedByHellUnit = 350;
        public static int wattConsumedByK9 = 250;
        public static int wattConsumedByMUFF = 350;
        public static int wattConsumedByPhytomining = 200;
        public static int wattConsumedByNSolution = 250;
        public static int wattConsumedByFENNEC = 250;


        public static int ransomCostT1 = 150;
        public static int ransomCostT2 = 250;
        public static int ransomCostT3 = 350;
        public static int ransomCostT4 = 500;
        public static int ransomCostT5 = 800;

        public static bool disableSolarFlareEffect = false;
        public static bool disableSkyMindSecurityStuff = false;
        public static bool androidsCanUseOrganicMedicine = false;
        public static float riskSecurisedSecuritySystemGetVirus = 0.25f;
        public static float riskCryptolockerScam = 0.25f;
        public static float riskRansomwareScam = 0.25f;
        public static int nbSecExplosiveVirusTakeToExplode = 45;

        public static int nbHoursMinServerRunningHotBeforeExplode = 4;
        public static int nbHoursMaxServerRunningHotBeforeExplode = 12;

        public static int nbHoursMinSkyCloudServerRunningHotBeforeExplode = 8;
        public static int nbHoursMaxSkyCloudServerRunningHotBeforeExplode = 24;

        public static bool androidNeedToEatMore = true;
        public static float percentageOfBatteryChargedEach6Sec = 0.05f;

        public static int ransomwareMinSilverToPayForBasTrait = 500;
        public static int ransomwareMaxSilverToPayForBasTrait = 2500;

        public static int ransomwareSilverToPayToRestoreSkillPerLevel = 175;

        public static int costPlayerHack = 1200;
        public static int costPlayerHackTemp = 150;
        public static int costPlayerVirus = 500;
        public static int costPlayerExplosiveVirus = 1000;

        public static int nbSecDurationTempHack = 40;

        public static Vector2 scrollPosition = Vector2.zero;

        public static float percentageOfSurrogateInAnotherFactionGroup = 0.35f;
        public static float percentageOfSurrogateInAnotherFactionGroupMin = 0.05f;

        public static void DoSettingsWindowContents(Rect inRect)
        {
            inRect.yMin += 15f;
            inRect.yMax -= 15f;

            var defaultColumnWidth = (inRect.width - 50);
            Listing_Standard list = new Listing_Standard() { ColumnWidth = defaultColumnWidth };


            var outRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            var scrollRect = new Rect(0f, 0f, inRect.width - 16f, inRect.height * 12f);
            Widgets.BeginScrollView(outRect, ref scrollPosition, scrollRect, true);

            list.Begin(scrollRect);

            list.ButtonImage(Tex.SettingsHeader, 850, 128);
            list.GapLine();
            list.Gap(10);
            GUI.color = Color.cyan;
            list.Label("ATPP_SettingsSectionGeneral".Translate());
            GUI.color = Color.white;
            list.Gap(10);
            list.GapLine();


            list.CheckboxLabeled("ATPP_SettingsVX0KeepBodyBackstory".Translate(), ref keepPuppetBackstory);

            if (Utils.ANDROIDTIERSGYNOID_LOADED)
            {
                list.Label("ATPP_SettingsChanceCreatedAndroidsAreMale".Translate((int)(percentageChanceMaleAndroidModel * 100)));
                percentageChanceMaleAndroidModel = list.Slider(percentageChanceMaleAndroidModel, 0.0f, 1.0f);
            }


            bool prevAllowT5ToWearClothes = allowT5ToWearClothes;
            list.CheckboxLabeled("ATPP_SettingsAllowT5ToWearClothes".Translate(), ref allowT5ToWearClothes);

            if(allowT5ToWearClothes != prevAllowT5ToWearClothes)
            {
                Utils.applyT5ClothesPolicy();
            }

            if (Utils.POWERPP_LOADED)
            {
                list.Label("ATPP_SettingsMaxAndroidByPortableLWPN".Translate(maxAndroidByPortableLWPN));
                maxAndroidByPortableLWPN = (int)list.Slider(maxAndroidByPortableLWPN, 1, 100);
            }


            list.CheckboxLabeled("ATPP_SettingsAllowAutoRepaint".Translate(), ref allowAutoRepaint);

            if(allowAutoRepaint)
                list.CheckboxLabeled("ATPP_SettingsAllowAutoRepaintPrisoned".Translate(), ref allowAutoRepaintForPrisoners);

            list.CheckboxLabeled("ATPP_SettingsAndroidsAreRare".Translate(), ref androidsAreRare);

            list.Label("ATPP_SettingsChanceGeneratedAndroidCanBePaintedOrRust".Translate((int)(chanceGeneratedAndroidCanBePaintedOrRust*100) ));
            chanceGeneratedAndroidCanBePaintedOrRust = list.Slider(chanceGeneratedAndroidCanBePaintedOrRust, 0.0f, 1.0f);

            list.CheckboxLabeled("ATPP_SettingsAndroidsCanRust".Translate(), ref androidsCanRust);

            if (androidsCanRust)
            {
                list.Label("ATPP_SettingsMinDaysBeforeAndroidsPaintingCanRust".Translate(minDaysAndroidPaintingCanRust));
                minDaysAndroidPaintingCanRust = (int)list.Slider(minDaysAndroidPaintingCanRust, 1, 200);

                list.Label("ATPP_SettingsMaxDaysBeforeAndroidsPaintingCanRust".Translate(maxDaysAndroidPaintingCanRust));
                maxDaysAndroidPaintingCanRust = (int)list.Slider(maxDaysAndroidPaintingCanRust, 1, 200);

                if (maxDaysAndroidPaintingCanRust < minDaysAndroidPaintingCanRust)
                    minDaysAndroidPaintingCanRust = maxDaysAndroidPaintingCanRust;
            }

            list.CheckboxLabeled("ATPP_SettingsRemoveSimpleMindedTraitOnUpload".Translate(), ref removeSimpleMindedTraitOnUpload);

            list.CheckboxLabeled("ATPP_SettingsAllowHumanDrugsFor".Translate(), ref allowHumanDrugsForAndroids);
            

            list.CheckboxLabeled("ATPP_SettingsAllowHumanDrugsForT3P".Translate(), ref allowHumanDrugsForT3PlusAndroids);

            list.CheckboxLabeled("ATPP_RemoveComfortNeedForT3T4".Translate(), ref removeComfortNeedForT3T4);

            list.Label("ATPP_SettingsMinHoursNaniteBankFramework".Translate(minHoursNaniteFramework));
            minHoursNaniteFramework = (int)list.Slider(minHoursNaniteFramework, 1, 200);

            list.Label("ATPP_SettingsMaxHoursNaniteBankFramework".Translate(maxHoursNaniteFramework));
            maxHoursNaniteFramework = (int)list.Slider(maxHoursNaniteFramework, 1, 200);

            if (maxHoursNaniteFramework < minHoursNaniteFramework)
                minHoursNaniteFramework = maxHoursNaniteFramework;


            list.CheckboxLabeled("ATPP_SettingsPreventM7T5AppearingInCharacterEditor".Translate(), ref preventM7T5AppearingInCharacterScreen);

            bool prevAndroidsCanConsumeLivingPlants = androidsCanConsumeLivingPlants;
            list.CheckboxLabeled("ATPP_SettingsAndroidsCanEatLivingPlants".Translate(), ref androidsCanConsumeLivingPlants);

            if (androidsCanConsumeLivingPlants != prevAndroidsCanConsumeLivingPlants)
            {
                Utils.applyLivingPlantPolicy();
            }

            if (androidsCanConsumeLivingPlants)
            {
                list.CheckboxLabeled("ATPP_SettingsHideAbilityToForceEatingLivingPlants".Translate(), ref hideMenuAllowingForceEatingLivingPlants);
            }

            list.CheckboxLabeled("ATPP_SettingsNotRemoveSkillsPassionForBasicAndroids".Translate(), ref notRemoveAllSkillPassionsForBasicAndroids);

            list.CheckboxLabeled("ATPP_SettingsNotRemoveAllTraitsOfBasicAndroids".Translate(), ref notRemoveAllTraitsFromT1T2);

            list.CheckboxLabeled("ATPP_SettingsAndroidsOnlyHealedByCrafter".Translate(), ref androidsCanOnlyBeHealedByCrafter);

            bool prevHideInactiveSurrogates = hideInactiveSurrogates;
            list.CheckboxLabeled("ATPP_SettingsHideInactiveSurrogates".Translate(), ref hideInactiveSurrogates);

            if(hideRemotelyControlledDeviceIcon != prevHideInactiveSurrogates)
            {
                if (Current.ProgramState == ProgramState.Playing)
                       Find.ColonistBar.MarkColonistsDirty();
            }

            bool prevDisableLowNetworkMalusInCaravan = disableLowNetworkMalusInCaravans;
            list.CheckboxLabeled("ATPP_SettingsDisableLowNetworkMalusInCaravans".Translate(), ref disableLowNetworkMalusInCaravans);

            if(prevDisableLowNetworkMalusInCaravan != disableLowNetworkMalusInCaravans)
            {
                if (disableLowNetworkMalusInCaravans)
                {
                    Utils.removeAllSlowNetworkHediff(true);
                }
            }


            bool prevDisableLowNetworkMalus = disableLowNetworkMalus;
            list.CheckboxLabeled("ATPP_SettingsDisableLowNetworkMalus".Translate(), ref disableLowNetworkMalus);

            if(prevDisableLowNetworkMalus != disableLowNetworkMalus)
            {
                if (disableLowNetworkMalus)
                {
                    Utils.removeAllSlowNetworkHediff();
                }
            }

            list.CheckboxLabeled(("ATPP_SettingsDisableServersAmbianceNoises".Translate())+("ATPP_SettingsAppliedToNextSaveLoading".Translate()), ref disableServersAmbiance);
            list.CheckboxLabeled(("ATPP_SettingsDisableServersAlarm".Translate()) + ("ATPP_SettingsAppliedToNextSaveLoading".Translate()), ref disableServersAlarm);

            bool prevDuringSolarFlaresAndroidsShouldBeDowned = duringSolarFlaresAndroidsShouldBeDowned;

            list.CheckboxLabeled("ATPP_SettingsDuringSolarFlareAndroidsShouldBeDowned".Translate(), ref duringSolarFlaresAndroidsShouldBeDowned);

            if (prevDuringSolarFlaresAndroidsShouldBeDowned != duringSolarFlaresAndroidsShouldBeDowned)
            {
                Utils.applySolarFlarePolicy();
            }

            list.CheckboxLabeled("ATPP_SettingsOtherFactionsCanUseSurrogate".Translate(), ref otherFactionsCanUseSurrogate);

            list.Label("ATPP_SettingsPercentageOfSurrogateInAnotherFactionGroupMin".Translate((int)(percentageOfSurrogateInAnotherFactionGroupMin * 100)));
            percentageOfSurrogateInAnotherFactionGroupMin = list.Slider(percentageOfSurrogateInAnotherFactionGroupMin, 0.0f, 1.0f);

            list.Label("ATPP_SettingsPercentageOfSurrogateInAnotherFactionGroup".Translate((int)(percentageOfSurrogateInAnotherFactionGroup * 100)));
            percentageOfSurrogateInAnotherFactionGroup = list.Slider(percentageOfSurrogateInAnotherFactionGroup, 0.0f, 1.0f);

            if (percentageOfSurrogateInAnotherFactionGroup < percentageOfSurrogateInAnotherFactionGroupMin)
                percentageOfSurrogateInAnotherFactionGroupMin = percentageOfSurrogateInAnotherFactionGroup;

            string buffNbSkillPointsPerSkillT1 = nbSkillPointsPerSkillT1.ToString();
            string buffNbSkillPointsPerSkillT2 = nbSkillPointsPerSkillT2.ToString();
            string buffNbSkillPointsPerSkillT3 = nbSkillPointsPerSkillT3.ToString();
            string buffNbSkillPointsPerSkillT4 = nbSkillPointsPerSkillT4.ToString();
            string buffNbSkillPointsPerSkillT5 = nbSkillPointsPerSkillT5.ToString();

            string buffNbSkillPointsPassionT1 = nbSkillPointsPassionT1.ToString();
            string buffNbSkillPointsPassionT2 = nbSkillPointsPassionT2.ToString();
            string buffNbSkillPointsPassionT3 = nbSkillPointsPassionT3.ToString();
            string buffNbSkillPointsPassionT4 = nbSkillPointsPassionT4.ToString();
            string buffNbSkillPointsPassionT5 = nbSkillPointsPassionT5.ToString();

            list.Label("ATPP_SettingsNbSkillPointsPerSkill".Translate("T1"));
            list.TextFieldNumeric(ref nbSkillPointsPerSkillT1, ref buffNbSkillPointsPerSkillT1, 1, 999999);

            list.Label("ATPP_SettingsNbSkillPointsPerSkill".Translate("T2"));
            list.TextFieldNumeric(ref nbSkillPointsPerSkillT2, ref buffNbSkillPointsPerSkillT2, 1, 999999);

            list.Label("ATPP_SettingsNbSkillPointsPerSkill".Translate("T3"));
            list.TextFieldNumeric(ref nbSkillPointsPerSkillT3, ref buffNbSkillPointsPerSkillT3, 1, 999999);

            list.Label("ATPP_SettingsNbSkillPointsPerSkill".Translate("T4"));
            list.TextFieldNumeric(ref nbSkillPointsPerSkillT4, ref buffNbSkillPointsPerSkillT4, 1, 999999);

            list.Label("ATPP_SettingsNbSkillPointsPerSkill".Translate("T5"));
            list.TextFieldNumeric(ref nbSkillPointsPerSkillT5, ref buffNbSkillPointsPerSkillT5, 1, 999999);

            list.Label("ATPP_SettingsPassionCost".Translate("T1"));
            list.TextFieldNumeric(ref nbSkillPointsPassionT1, ref buffNbSkillPointsPassionT1, 1, 999999);

            list.Label("ATPP_SettingsPassionCost".Translate("T2"));
            list.TextFieldNumeric(ref nbSkillPointsPassionT2, ref buffNbSkillPointsPassionT2, 1, 999999);

            list.Label("ATPP_SettingsPassionCost".Translate("T3"));
            list.TextFieldNumeric(ref nbSkillPointsPassionT3, ref buffNbSkillPointsPassionT3, 1, 999999);

            list.Label("ATPP_SettingsPassionCost".Translate("T4"));
            list.TextFieldNumeric(ref nbSkillPointsPassionT4, ref buffNbSkillPointsPassionT4, 1, 999999);

            list.Label("ATPP_SettingsPassionCost".Translate("T5"));
            list.TextFieldNumeric(ref nbSkillPointsPassionT5, ref buffNbSkillPointsPassionT5, 1, 999999);



            list.Label("ATPP_SettingsNbSlotAddedForSkillServers".Translate("I-100", skillSlotsForOldSkillServers));
            skillSlotsForOldSkillServers = (int)list.Slider(skillSlotsForOldSkillServers, 1, 1000);

            list.Label("ATPP_SettingsNbSlotAddedForSkillServers".Translate("I-300", skillSlotsForBasicSkillServers));
            skillSlotsForBasicSkillServers = (int)list.Slider(skillSlotsForBasicSkillServers, 1, 1000);

            list.Label("ATPP_SettingsNbSlotAddedForSkillServers".Translate("I-500", skillSlotsForAdvancedSkillServers));
            skillSlotsForAdvancedSkillServers = (int)list.Slider(skillSlotsForAdvancedSkillServers, 1, 1000);


            list.Label("ATPP_SettingsNbPointsProducedForSkillServers".Translate("I-100", skillNbpGeneratedOld));
            skillNbpGeneratedOld = (int)list.Slider(skillNbpGeneratedOld, 1, 100);

            list.Label("ATPP_SettingsNbPointsProducedForSkillServers".Translate("I-300", skillNbpGeneratedBasic));
            skillNbpGeneratedBasic = (int)list.Slider(skillNbpGeneratedBasic, 1, 100);

            list.Label("ATPP_SettingsNbPointsProducedForSkillServers".Translate("I-500", skillNbpGeneratedAdvanced));
            skillNbpGeneratedAdvanced = (int)list.Slider(skillNbpGeneratedAdvanced, 1, 100);

            

            //ATPP_SettingsRiskGetLittleVirusEvenWithSecurityServers*
            list.CheckboxLabeled("ATPP_SettingsDisableSolarFlareImpactOnAndroids".Translate(), ref disableSolarFlareEffect);
            list.CheckboxLabeled("ATPP_SettingsFoodGenerateLessEnergy".Translate(), ref androidNeedToEatMore);
            GUI.color = Color.yellow;
            list.Label("ATPP_SettingsDefaultAndroidGeneratorMode".Translate());
            GUI.color = Color.white;
            if (list.RadioButton("ATPP_SettingsDefaultAndroidGeneratorModeBiomass".Translate(), (defaultGeneratorMode == 1)))
                defaultGeneratorMode = 1;
            if (list.RadioButton("ATPP_SettingsDefaultAndroidGeneratorModeBattery".Translate(), (defaultGeneratorMode == 2)))
                defaultGeneratorMode = 2;

            list.Label("ATPP_SettingsPercentageOfAndroidBatteryReloadedEachXSec".Translate((int)(percentageOfBatteryChargedEach6Sec * 100)));
            percentageOfBatteryChargedEach6Sec = list.Slider(percentageOfBatteryChargedEach6Sec, 0.01f, 1.0f);

            list.Label("ATPP_SettingsNbHoursMinSkyCloudServerRunningHotBeforeExplode".Translate(nbHoursMinSkyCloudServerRunningHotBeforeExplode));
            nbHoursMinSkyCloudServerRunningHotBeforeExplode = (int)list.Slider(nbHoursMinSkyCloudServerRunningHotBeforeExplode, 1, 200);

            list.Label("ATPP_SettingsNbHoursMaxSkyCloudServerRunningHotBeforeExplode".Translate(nbHoursMaxSkyCloudServerRunningHotBeforeExplode));
            nbHoursMaxSkyCloudServerRunningHotBeforeExplode = (int)list.Slider(nbHoursMaxSkyCloudServerRunningHotBeforeExplode, 1, 200);

            if (nbHoursMaxSkyCloudServerRunningHotBeforeExplode < nbHoursMinSkyCloudServerRunningHotBeforeExplode)
                nbHoursMinSkyCloudServerRunningHotBeforeExplode = nbHoursMaxSkyCloudServerRunningHotBeforeExplode;


            list.Label("ATPP_SettingsNbHoursMinServerRunningHotBeforeExplode".Translate(nbHoursMinServerRunningHotBeforeExplode));
            nbHoursMinServerRunningHotBeforeExplode = (int)list.Slider(nbHoursMinServerRunningHotBeforeExplode, 1, 200);

            list.Label("ATPP_SettingsNbHoursMaxServerRunningHotBeforeExplode".Translate(nbHoursMaxServerRunningHotBeforeExplode));
            nbHoursMaxServerRunningHotBeforeExplode = (int)list.Slider(nbHoursMaxServerRunningHotBeforeExplode, 1, 200);

            if (nbHoursMaxServerRunningHotBeforeExplode < nbHoursMinServerRunningHotBeforeExplode)
                nbHoursMinServerRunningHotBeforeExplode = nbHoursMaxServerRunningHotBeforeExplode;

            list.GapLine();
            list.Label("ATPP_SettingsMindUploadDuration".Translate((int)(mindUploadHour)));
            mindUploadHour = (int) list.Slider(mindUploadHour, 1, 48);

            list.Label("ATPP_SettingsNanitePercentageFail".Translate((int)(percentageNanitesFail * 100)));
            percentageNanitesFail = list.Slider(percentageNanitesFail, 0.0f, 1.0f);


            string buffWattConsumedByT1 = wattConsumedByT1.ToString();
            string buffWattConsumedByT2 = wattConsumedByT2.ToString();
            string buffWattConsumedByT3 = wattConsumedByT3.ToString();
            string buffWattConsumedByT4 = wattConsumedByT4.ToString();
            string buffWattConsumedByT5 = wattConsumedByT5.ToString();
            string buffWattConsumedByM7 = wattConsumedByM7.ToString();
            string buffWattConsumedByHellUnit = wattConsumedByHellUnit.ToString();
            string buffWattConsumedByMUFF = wattConsumedByMUFF.ToString();
            string buffWattConsumedByNSolution = wattConsumedByNSolution.ToString();
            string buffWattConsumedByK9 = wattConsumedByK9.ToString();
            string buffWattConsumedByPhytomining = wattConsumedByPhytomining.ToString();
            string buffWattConsumedByFENNEC = wattConsumedByFENNEC.ToString();

            list.Label("ATPP_SettingsCharchingStationWattIncrease".Translate("T1"));
            list.TextFieldNumeric(ref wattConsumedByT1, ref buffWattConsumedByT1, 1, 999999);

            list.Label("ATPP_SettingsCharchingStationWattIncrease".Translate("T2"));
            list.TextFieldNumeric(ref wattConsumedByT2, ref buffWattConsumedByT2, 1, 999999);

            list.Label("ATPP_SettingsCharchingStationWattIncrease".Translate("T3"));
            list.TextFieldNumeric(ref wattConsumedByT3, ref buffWattConsumedByT3, 1, 999999);

            list.Label("ATPP_SettingsCharchingStationWattIncrease".Translate("T4"));
            list.TextFieldNumeric(ref wattConsumedByT4, ref buffWattConsumedByT4, 1, 999999);

            list.Label("ATPP_SettingsCharchingStationWattIncrease".Translate("T5"));
            list.TextFieldNumeric(ref wattConsumedByT5, ref buffWattConsumedByT5, 1, 999999);

            list.Label("ATPP_SettingsCharchingStationWattIncrease".Translate("M7"));
            list.TextFieldNumeric(ref wattConsumedByM7, ref buffWattConsumedByM7, 1, 999999);

            list.Label("ATPP_SettingsCharchingStationWattIncrease".Translate("HellDrone"));
            list.TextFieldNumeric(ref wattConsumedByHellUnit, ref buffWattConsumedByHellUnit, 1, 999999);

            list.Label("ATPP_SettingsCharchingStationWattIncrease".Translate("MUFF"));
            list.TextFieldNumeric(ref wattConsumedByMUFF, ref buffWattConsumedByMUFF, 1, 999999);

            list.Label("ATPP_SettingsCharchingStationWattIncrease".Translate("K9"));
            list.TextFieldNumeric(ref wattConsumedByK9, ref buffWattConsumedByK9, 1, 999999);

            list.Label("ATPP_SettingsCharchingStationWattIncrease".Translate("PhytoMining"));
            list.TextFieldNumeric(ref wattConsumedByPhytomining, ref buffWattConsumedByPhytomining, 1, 999999);

            list.Label("ATPP_SettingsCharchingStationWattIncrease".Translate("NSolution"));
            list.TextFieldNumeric(ref wattConsumedByNSolution, ref buffWattConsumedByNSolution, 1, 999999);

            list.Label("ATPP_SettingsCharchingStationWattIncrease".Translate("NSolution"));
            list.TextFieldNumeric(ref wattConsumedByFENNEC, ref buffWattConsumedByFENNEC, 1, 999999);

            


            list.GapLine();
            list.Gap(10);
            GUI.color = Color.cyan;
            list.Label("ATPP_SettingsSectionSkills".Translate());
            GUI.color = Color.white;
            list.Gap(10);
            list.GapLine();


            list.CheckboxLabeled("ATPP_SettingsRandomSkillsMode".Translate(), ref basicAndroidsRandomSKills);

            if (!basicAndroidsRandomSKills)
            {
                GUI.color = Color.yellow;
                list.Label("T1 :");
                GUI.color = Color.white;

                //T1 default skills 
                list.Label("Shooting".Translate() + " " + defaultSkillT1Shoot + "/20");
                defaultSkillT1Shoot = (int)list.Slider(defaultSkillT1Shoot, 0, 20);

                list.Label("Melee".Translate() + " " + defaultSkillT1Melee + "/20");
                defaultSkillT1Melee = (int)list.Slider(defaultSkillT1Melee, 0, 20);

                list.Label("Construction".Translate() + " " + defaultSkillT1Construction + "/20");
                defaultSkillT1Construction = (int)list.Slider(defaultSkillT1Construction, 0, 20);

                list.Label("Mining".Translate() + " " + defaultSkillT1Mining + "/20");
                defaultSkillT1Mining = (int)list.Slider(defaultSkillT1Mining, 0, 20);

                list.Label("Cooking".Translate() + " " + defaultSkillT1Cooking + "/20");
                defaultSkillT1Cooking = (int)list.Slider(defaultSkillT1Cooking, 0, 20);

                list.Label("Plants".Translate() + " " + defaultSkillT1Plants + "/20");
                defaultSkillT1Plants = (int)list.Slider(defaultSkillT1Plants, 0, 20);

                list.Label("Animals".Translate() + " " + defaultSkillT1Animals + "/20");
                defaultSkillT1Animals = (int)list.Slider(defaultSkillT1Animals, 0, 20);

                list.Label("Crafting".Translate() + " " + defaultSkillT1Crafting + "/20");
                defaultSkillT1Crafting = (int)list.Slider(defaultSkillT1Crafting, 0, 20);

                list.Label("Artistic".Translate() + " " + defaultSkillT1Artistic + "/20");
                defaultSkillT1Artistic = (int)list.Slider(defaultSkillT1Artistic, 0, 20);

                list.Label("Medicine".Translate() + " " + defaultSkillT1Medical + "/20");
                defaultSkillT1Medical = (int)list.Slider(defaultSkillT1Medical, 0, 20);

                list.Label("Social".Translate() + " " + defaultSkillT1Social + "/20");
                defaultSkillT1Social = (int)list.Slider(defaultSkillT1Social, 0, 20);

                list.Label("Intellectual".Translate() + " " + defaultSkillT1Intellectual + "/20");
                defaultSkillT1Intellectual = (int)list.Slider(defaultSkillT1Intellectual, 0, 20);


                GUI.color = Color.yellow;
                list.Label("T2 :");
                GUI.color = Color.white;

                list.Label("Shooting".Translate() + " " + defaultSkillT2Shoot + "/20");
                defaultSkillT2Shoot = (int)list.Slider(defaultSkillT2Shoot, 0, 20);

                list.Label("Melee".Translate() + " " + defaultSkillT2Melee + "/20");
                defaultSkillT2Melee = (int)list.Slider(defaultSkillT2Melee, 0, 20);

                list.Label("Construction".Translate() + " " + defaultSkillT2Construction + "/20");
                defaultSkillT2Construction = (int)list.Slider(defaultSkillT2Construction, 0, 20);

                list.Label("Mining".Translate() + " " + defaultSkillT2Mining + "/20");
                defaultSkillT2Mining = (int)list.Slider(defaultSkillT2Mining, 0, 20);

                list.Label("Cooking".Translate() + " " + defaultSkillT2Cooking + "/20");
                defaultSkillT2Cooking = (int)list.Slider(defaultSkillT2Cooking, 0, 20);

                list.Label("Plants".Translate() + " " + defaultSkillT2Plants + "/20");
                defaultSkillT2Plants = (int)list.Slider(defaultSkillT2Plants, 0, 20);

                list.Label("Animals".Translate() + " " + defaultSkillT2Animals + "/20");
                defaultSkillT2Animals = (int)list.Slider(defaultSkillT2Animals, 0, 20);

                list.Label("Crafting".Translate() + " " + defaultSkillT2Crafting + "/20");
                defaultSkillT2Crafting = (int)list.Slider(defaultSkillT2Crafting, 0, 20);

                list.Label("Artistic".Translate() + " " + defaultSkillT2Artistic + "/20");
                defaultSkillT2Artistic = (int)list.Slider(defaultSkillT2Artistic, 0, 20);

                list.Label("Medicine".Translate() + " " + defaultSkillT2Medical + "/20");
                defaultSkillT2Medical = (int)list.Slider(defaultSkillT2Medical, 0, 20);

                list.Label("Social".Translate() + " " + defaultSkillT2Social + "/20");
                defaultSkillT2Social = (int)list.Slider(defaultSkillT2Social, 0, 20);

                list.Label("Intellectual".Translate() + " " + defaultSkillT2Intellectual + "/20");
                defaultSkillT2Intellectual = (int)list.Slider(defaultSkillT2Intellectual, 0, 20);
            }


            list.Gap(3);
            list.GapLine();
            list.Gap(10);
            GUI.color = Color.cyan;
            list.Label("ATPP_SettingsSectionSkyCloud".Translate());
            GUI.color = Color.white;
            list.Gap(10);
            list.GapLine();

            GUI.color = Color.yellow;
            list.Label("ATPP_SettingsUploadMindToSkyCloudCoreMode".Translate());
            GUI.color = Color.white;
            if (list.RadioButton("ATTP_SettingsUploadMindToSkyCloudCoreModeNothing".Translate(), (skyCloudUploadModeForSourceMind == 0)))
                skyCloudUploadModeForSourceMind = 0;
            if (list.RadioButton("ATTP_SettingsUploadMindToSkyCloudCoreModeVX0".Translate(), (skyCloudUploadModeForSourceMind == 1)))
                skyCloudUploadModeForSourceMind = 1;
            if (list.RadioButton("ATTP_SettingsUploadMindToSkyCloudCoreModeLethal".Translate(), (skyCloudUploadModeForSourceMind == 2)))
                skyCloudUploadModeForSourceMind = 2;

            list.Gap();

            list.Label("ATPP_SettingsMindUploadToSkyCloudDuration".Translate((int)(mindUploadToSkyCloudHours)));
            mindUploadToSkyCloudHours = (int)list.Slider(mindUploadToSkyCloudHours, 1, 72);

            list.Label("ATPP_SettingsSecToBootASkyCloudCore".Translate(secToBootSkyCloudCore));
            secToBootSkyCloudCore = (int)list.Slider(secToBootSkyCloudCore, 1, 300);

            list.Label("ATPP_SettingsMinDurationOfMindMentalBreak".Translate(minDurationMentalBreakOfDigitisedMinds));
            minDurationMentalBreakOfDigitisedMinds = (int)list.Slider(minDurationMentalBreakOfDigitisedMinds, 1, 100);

            list.Label("ATPP_SettingsMaxDurationOfMindMentalBreak".Translate(maxDurationMentalBreakOfDigitisedMinds));
            maxDurationMentalBreakOfDigitisedMinds = (int)list.Slider(maxDurationMentalBreakOfDigitisedMinds, 1, 100);

            if (maxDurationMentalBreakOfDigitisedMinds < minDurationMentalBreakOfDigitisedMinds)
                minDurationMentalBreakOfDigitisedMinds = maxDurationMentalBreakOfDigitisedMinds;

            list.Label("ATPP_SettingsMindReplicationDuration".Translate((int)(mindReplicationHours)));
            mindReplicationHours = (int)list.Slider(mindReplicationHours, 1, 72);

            //ATPP_SettingsSkyCloudBuffPerAssistingMind
            list.Label("ATPP_SettingsSkyCloudBuffPerAssistingMind".Translate(nbMoodPerAssistingMinds));
            nbMoodPerAssistingMinds = (int)list.Slider(nbMoodPerAssistingMinds, 1, 20);

            list.CheckboxLabeled("ATPP_SettingsHideRemotelyControlledIcon".Translate(), ref hideRemotelyControlledDeviceIcon);

            string buffPowerConsumedByStoredMind = powerConsumedByStoredMind.ToString();

            list.Label("ATPP_SettingsPowerConsumedByStoredMind".Translate());
            list.TextFieldNumeric(ref powerConsumedByStoredMind, ref buffPowerConsumedByStoredMind, 1, 999999);

            list.CheckboxLabeled("ATPP_SettingsDisableAbilityOfSkyCloudServersToTalk".Translate(), ref disableAbilitySkyCloudServerToTalk);

            list.Gap(3);
            list.GapLine();
            list.Gap(10);
            GUI.color = Color.cyan;
            list.Label("ATPP_SettingsSectionSecurity".Translate());
            GUI.color = Color.white;
            list.Gap(10);
            list.GapLine();


            list.CheckboxLabeled("ATPP_SettingsDisableSecurityStuff".Translate(), ref disableSkyMindSecurityStuff);

            if (!disableSkyMindSecurityStuff)
            {
                list.Label("ATPP_SettingsMindMigrationCloudDuration".Translate((int)(mindSkyCloudMigrationHours)));
                mindSkyCloudMigrationHours = (int)list.Slider(mindSkyCloudMigrationHours, 1, 72);

                list.Label("ATPP_SettingsMinNbHoursLiteHackingDeviceAttackLast".Translate((int)(nbHourLiteHackingDeviceAttackLastMin)));
                nbHourLiteHackingDeviceAttackLastMin = (int)list.Slider(nbHourLiteHackingDeviceAttackLastMin, 1, 100);

                list.Label("ATPP_SettingsMaxNbHoursLiteHackingDeviceAttackLast".Translate((int)(nbHourLiteHackingDeviceAttackLastMax)));
                nbHourLiteHackingDeviceAttackLastMax = (int)list.Slider(nbHourLiteHackingDeviceAttackLastMax, 1, 100);

                string buffCostPlayerVirus = costPlayerVirus.ToString();
                string buffCostPlayerExplosiveVirus = costPlayerExplosiveVirus.ToString();
                string buffCostPlayerHackTemp = costPlayerHackTemp.ToString();
                string buffCostPlayerHack = costPlayerHack.ToString();

                list.Label("ATPP_SettingsCostHackingPoints".Translate("ATPP_SettingsCostVirus".Translate()));
                list.TextFieldNumeric(ref costPlayerVirus, ref buffCostPlayerVirus, 1, 999999);

                list.Label("ATPP_SettingsCostHackingPoints".Translate("ATPP_SettingsCostVirusExplosive".Translate()));
                list.TextFieldNumeric(ref costPlayerExplosiveVirus, ref buffCostPlayerExplosiveVirus, 1, 999999);

                list.Label("ATPP_SettingsCostHackingPoints".Translate("ATPP_SettingsCostHackTemp".Translate()));
                list.TextFieldNumeric(ref costPlayerHackTemp, ref buffCostPlayerHackTemp, 1, 999999);

                list.Label("ATPP_SettingsCostHackingPoints".Translate("ATPP_SettingsCostHack".Translate()));
                list.TextFieldNumeric(ref costPlayerHack, ref buffCostPlayerHack, 1, 999999);

                //costPlayerVirus

                list.Label("ATPP_SettingsTempHackDuration".Translate((int)(nbSecDurationTempHack * 100)));
                nbSecDurationTempHack = (int)list.Slider(nbSecDurationTempHack, 1, 600);

                string buffRansomwareMinSilverToPayForBasTrait = ransomwareMinSilverToPayForBasTrait.ToString();
                string buffRansomwareMaxSilverToPayForBasTrait = ransomwareMaxSilverToPayForBasTrait.ToString();
                string buffRansomwareSilverToPayToRestoreSkillPerLevel = ransomwareSilverToPayToRestoreSkillPerLevel.ToString();

                list.Label("ATPP_SettingsMinSilverToRemoveBadTrait".Translate());
                list.TextFieldNumeric(ref ransomwareMinSilverToPayForBasTrait, ref buffRansomwareMinSilverToPayForBasTrait, 1, 999999);

                list.Label("ATPP_SettingsMaxSilverToRemoveBadTrait".Translate());
                list.TextFieldNumeric(ref ransomwareMaxSilverToPayForBasTrait, ref buffRansomwareMaxSilverToPayForBasTrait, 1, 999999);

                list.Label("ATPP_SettingsRansomwareSilverPerSkillLevel".Translate());
                list.TextFieldNumeric(ref ransomwareSilverToPayToRestoreSkillPerLevel, ref buffRansomwareSilverToPayToRestoreSkillPerLevel, 1, 999999);



                list.Label("ATPP_SettingsRiskFactionNotRemoveRansomwareEffect".Translate((int)(riskRansomwareScam * 100)));
                riskRansomwareScam = list.Slider(riskRansomwareScam, 0.01f, 1.0f);

                list.Label("ATPP_SettingsRiskGetLittleVirusEvenWithSecurityServers".Translate((int)(riskSecurisedSecuritySystemGetVirus * 100)));
                riskSecurisedSecuritySystemGetVirus = list.Slider(riskSecurisedSecuritySystemGetVirus, 0.0f, 1.0f);

                string buffRansomCostT1 = ransomCostT1.ToString();
                string buffRansomCostT2 = ransomCostT2.ToString();
                string buffRansomCostT3 = ransomCostT3.ToString();
                string buffRansomCostT4 = ransomCostT4.ToString();
                string buffRansomCostT5 = ransomCostT5.ToString();

                list.Label("ATPP_SettingsRansomCost".Translate("T1"));
                list.TextFieldNumeric(ref ransomCostT1, ref buffRansomCostT1, 1, 999999);

                list.Label("ATPP_SettingsRansomCost".Translate("T2"));
                list.TextFieldNumeric(ref ransomCostT2, ref buffRansomCostT2, 1, 999999);

                list.Label("ATPP_SettingsRansomCost".Translate("T3"));
                list.TextFieldNumeric(ref ransomCostT3, ref buffRansomCostT3, 1, 999999);

                list.Label("ATPP_SettingsRansomCost".Translate("T4"));
                list.TextFieldNumeric(ref ransomCostT4, ref buffRansomCostT4, 1, 999999);

                list.Label("ATPP_SettingsRansomCost".Translate("T5"));
                list.TextFieldNumeric(ref ransomCostT5, ref buffRansomCostT5, 1, 999999);


                list.Label("ATPP_SettingsRiskFactionNotRemoveCryptolocker".Translate((int)(riskCryptolockerScam * 100)));
                riskCryptolockerScam = list.Slider(riskCryptolockerScam, 0.01f, 1.0f);


                list.CheckboxLabeled("ATPP_SettingsAndroidCanUseOrganicMedicine".Translate(), ref androidsCanUseOrganicMedicine);

                //ATPP_SettingsDelayExplosiveVirus
                list.Label("ATPP_SettingsDelayExplosiveVirus".Translate(nbSecExplosiveVirusTakeToExplode));
                nbSecExplosiveVirusTakeToExplode = (int)list.Slider(nbSecExplosiveVirusTakeToExplode, 1, 240);

                list.Label("ATPP_SettingsNbSlotAddedForSecurityServers".Translate("I-100", securitySlotForOldSecurityServers));
                securitySlotForOldSecurityServers = (int)list.Slider(securitySlotForOldSecurityServers, 1, 100);

                list.Label("ATPP_SettingsNbSlotAddedForSecurityServers".Translate("I-300", securitySlotForBasicSecurityServers));
                securitySlotForBasicSecurityServers = (int)list.Slider(securitySlotForBasicSecurityServers, 1, 100);

                list.Label("ATPP_SettingsNbSlotAddedForSecurityServers".Translate("I-500", securitySlotForAdvancedSecurityServers));
                securitySlotForAdvancedSecurityServers = (int)list.Slider(securitySlotForAdvancedSecurityServers, 1, 100);


                list.Label("ATPP_SettingsNbSlotAddedForHackingServers".Translate("I-100", hackingSlotsForOldHackingServers));
                hackingSlotsForOldHackingServers = (int)list.Slider(hackingSlotsForOldHackingServers, 1, 1000);

                list.Label("ATPP_SettingsNbSlotAddedForHackingServers".Translate("I-300", hackingSlotsForBasicHackingServers));
                hackingSlotsForBasicHackingServers = (int)list.Slider(hackingSlotsForBasicHackingServers, 1, 1000);

                list.Label("ATPP_SettingsNbSlotAddedForHackingServers".Translate("I-500", hackingSlotsForAdvancedHackingServers));
                hackingSlotsForAdvancedHackingServers = (int)list.Slider(hackingSlotsForAdvancedHackingServers, 1, 1000);



                list.Label("ATPP_SettingsNbPointsProducedForHackingServers".Translate("I-100", hackingNbpGeneratedOld));
                hackingNbpGeneratedOld = (int)list.Slider(hackingNbpGeneratedOld, 1, 100);

                list.Label("ATPP_SettingsNbPointsProducedForHackingServers".Translate("I-300", hackingNbpGeneratedBasic));
                hackingNbpGeneratedBasic = (int)list.Slider(hackingNbpGeneratedBasic, 1, 100);

                list.Label("ATPP_SettingsNbPointsProducedForHackingServers".Translate("I-500", hackingNbpGeneratedAdvanced));
                hackingNbpGeneratedAdvanced = (int)list.Slider(hackingNbpGeneratedAdvanced, 1, 100);

            }

            list.Gap(3);
            list.GapLine();
            list.Gap(10);
            GUI.color = Color.cyan;
            list.Label("ATPP_SettingsSectionVX3".Translate());
            GUI.color = Color.white;
            list.Gap(10);
            list.GapLine();

            list.Label("ATPP_SettingsVX3MaxSurrogateControllableAtOnce".Translate((int)(VX3MaxSurrogateControllableAtOnce)));
            VX3MaxSurrogateControllableAtOnce = (int)list.Slider(VX3MaxSurrogateControllableAtOnce, 1, 50);

            list.Gap(3);
            list.GapLine();
            list.Gap(10);
            GUI.color = Color.cyan;
            list.Label("ATPP_SettingsSectionVX2".Translate());
            GUI.color = Color.white;
            list.Gap(10);
            list.GapLine();


            list.Label("ATPP_SettingsMindPermutationDuration".Translate((int)(mindPermutationHours)));
            mindPermutationHours = (int)list.Slider(mindPermutationHours, 1, 72);

            list.Label("ATPP_SettingsMindDuplicationDuration".Translate((int)(mindDuplicationHours)));
            mindDuplicationHours = (int)list.Slider(mindDuplicationHours, 1, 72);


            

            list.End();
            Widgets.EndScrollView();
            //settings.Write();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            
            Scribe_Values.Look<bool>(ref androidsCanOnlyBeHealedByCrafter, "androidsCanOnlyBeHealedByCrafter", false);
            Scribe_Values.Look<int>(ref mindSkyCloudMigrationHours, "mindSkyCloudMigrationHours", 4);
            Scribe_Values.Look<int>(ref mindUploadHour, "mindUploadHour", 3);
            Scribe_Values.Look<int>(ref mindReplicationHours, "mindReplicationHours", 2);
            Scribe_Values.Look<int>(ref mindDuplicationHours, "mindDuplicationHours", 8);
            Scribe_Values.Look<int>(ref mindUploadToSkyCloudHours, "mindUploadToSkyCloudHours", 8);
            Scribe_Values.Look<int>(ref mindPermutationHours, "mindPermutationHours", 4);

            Scribe_Values.Look<int>(ref defaultGeneratorMode, "defaultGeneratorMode", 1);
            Scribe_Values.Look<float>(ref percentageNanitesFail, "percentageNanitesFail", 0.08f);
            Scribe_Values.Look<float>(ref percentageOfBatteryChargedEach6Sec, "percentageOfBatteryChargedEach6Sec", 0.05f);

            Scribe_Values.Look<int>(ref wattConsumedByT1, "wattConsumedByT1", 90);
            Scribe_Values.Look<int>(ref wattConsumedByT2, "wattConsumedByT2", 150);
            Scribe_Values.Look<int>(ref wattConsumedByT3, "wattConsumedByT3", 200);
            Scribe_Values.Look<int>(ref wattConsumedByT4, "wattConsumedByT4", 250);
            Scribe_Values.Look<int>(ref wattConsumedByT5, "wattConsumedByT5", 350);
            Scribe_Values.Look<int>(ref wattConsumedByM7, "wattConsumedByM7", 500);
            Scribe_Values.Look<int>(ref wattConsumedByHellUnit, "wattConsumedByHellDrone", 350);
            Scribe_Values.Look<int>(ref wattConsumedByK9, "wattConsumedByK9", 250);
            Scribe_Values.Look<int>(ref wattConsumedByPhytomining, "wattConsumedByPhytomining", 200);
            Scribe_Values.Look<int>(ref wattConsumedByNSolution, "wattConsumedByNSolution", 250);
            Scribe_Values.Look<int>(ref wattConsumedByMUFF, "wattConsumedByMUFF", 350);
            Scribe_Values.Look<int>(ref wattConsumedByFENNEC, "wattConsumedByFENNEC", 250);


            Scribe_Values.Look<int>(ref nbHoursMaxServerRunningHotBeforeExplode, "nbHoursMaxServerRunningHotBeforeExplode", 4);
            Scribe_Values.Look<int>(ref nbHoursMinServerRunningHotBeforeExplode, "nbHoursMinServerRunningHotBeforeExplode", 12);

            Scribe_Values.Look<int>(ref nbHoursMaxSkyCloudServerRunningHotBeforeExplode, "nbHoursMaxSkyCloudServerRunningHotBeforeExplode", 12);
            Scribe_Values.Look<int>(ref nbHoursMinSkyCloudServerRunningHotBeforeExplode, "nbHoursMinSkyCloudServerRunningHotBeforeExplode", 48);

            Scribe_Values.Look<float>(ref riskSecurisedSecuritySystemGetVirus, "riskSecurisedSecuritySystemGetVirus", 0.25f);

            Scribe_Values.Look<bool>(ref androidNeedToEatMore, "androidNeedToEatMore", true);
            Scribe_Values.Look<bool>(ref androidsCanUseOrganicMedicine, "androidsCanUseOrganicMedicine", false);

            Scribe_Values.Look<int>(ref nbSecExplosiveVirusTakeToExplode, "nbSecExplosiveVirusTakeToExplode", 45);
            Scribe_Values.Look<float>(ref riskCryptolockerScam, "riskCryptolockerScam", 0.25f);
            Scribe_Values.Look<float>(ref riskRansomwareScam, "riskRansomwareScam", 0.25f);
            Scribe_Values.Look<float>(ref percentageOfSurrogateInAnotherFactionGroup, "percentageOfSurrogateInAnotherFactionGroup", 0.35f);
            Scribe_Values.Look<float>(ref percentageOfSurrogateInAnotherFactionGroupMin, "percentageOfSurrogateInAnotherFactionGroupMin", 0.05f);

            Scribe_Values.Look<bool>(ref otherFactionsCanUseSurrogate, "otherFactionsCanUseSurrogate", true);

            Scribe_Values.Look<int>(ref ransomCostT1, "ransomCostT1", 150);
            Scribe_Values.Look<int>(ref ransomCostT2, "ransomCostT2", 250);
            Scribe_Values.Look<int>(ref ransomCostT3, "ransomCostT3", 350);
            Scribe_Values.Look<int>(ref ransomCostT4, "ransomCostT4", 500);
            Scribe_Values.Look<int>(ref ransomCostT5, "ransomCostT5", 800);

            Scribe_Values.Look<int>(ref costPlayerExplosiveVirus, "costPlayerExplosiveVirus", 1000);
            Scribe_Values.Look<int>(ref costPlayerVirus, "costPlayerVirus", 500);
            Scribe_Values.Look<int>(ref costPlayerHack, "costPlayerHack", 1500);
            Scribe_Values.Look<int>(ref costPlayerHackTemp, "costPlayerHackTemp", 150);

            Scribe_Values.Look<int>(ref nbSecDurationTempHack, "nbSecDurationTempHack", 40);

            Scribe_Values.Look<bool>(ref duringSolarFlaresAndroidsShouldBeDowned, "duringSolarFlaresAndroidsShouldBeDowned", false);

            Scribe_Values.Look<bool>(ref disableServersAlarm, "disableServersAlarm", true);
            Scribe_Values.Look<bool>(ref disableServersAmbiance, "disableServersAmbiance", false);
            Scribe_Values.Look<bool>(ref disableLowNetworkMalus, "disableLowNetworkMalus", false);
            Scribe_Values.Look<bool>(ref disableLowNetworkMalusInCaravans, "disableLowNetworkMalusInCaravans", true);
            Scribe_Values.Look<bool>(ref hideRemotelyControlledDeviceIcon, "hideRemotelyControlledDeviceIcon", false);
            Scribe_Values.Look<int>(ref nbMoodPerAssistingMinds, "nbMoodPerAssistingMinds", 1);

            Scribe_Values.Look<int>(ref securitySlotForOldSecurityServers, "securitySlotForOldSecurityServers", 2);
            Scribe_Values.Look<int>(ref securitySlotForBasicSecurityServers, "securitySlotForBasicSecurityServers", 5);
            Scribe_Values.Look<int>(ref securitySlotForAdvancedSecurityServers, "securitySlotForAdvancedSecurityServers", 10);

            Scribe_Values.Look<int>(ref hackingSlotsForOldHackingServers, "hackingSlotsForOldHackingServers", 50);
            Scribe_Values.Look<int>(ref hackingSlotsForBasicHackingServers, "hackingSlotsForBasicHackingServers", 100);
            Scribe_Values.Look<int>(ref hackingSlotsForAdvancedHackingServers, "hackingSlotsForAdvancedHackingServers", 200);

            Scribe_Values.Look<int>(ref nbHourLiteHackingDeviceAttackLastMin, "nbHourLiteHackingDeviceAttackLastMin", 1);
            Scribe_Values.Look<int>(ref nbHourLiteHackingDeviceAttackLastMax, "nbHourLiteHackingDeviceAttackLastMax", 8);

            Scribe_Values.Look<bool>(ref hideInactiveSurrogates, "hideInactiveSurrogates", true);

            Scribe_Values.Look<int>(ref minDurationMentalBreakOfDigitisedMinds, "minDurationMentalBreakOfDigitisedMinds", 4);
            Scribe_Values.Look<int>(ref maxDurationMentalBreakOfDigitisedMinds, "maxDurationMentalBreakOfDigitisedMinds", 36);

            Scribe_Values.Look<int>(ref secToBootSkyCloudCore, "secToBootSkyCloudCore", 30);
            Scribe_Values.Look<int>(ref powerConsumedByStoredMind, "powerConsumedByStoredMind", 350);
            Scribe_Values.Look<bool>(ref disableAbilitySkyCloudServerToTalk, "disableAbilitySkyCloudServerToTalk", false);

            Scribe_Values.Look<bool>(ref notRemoveAllTraitsFromT1T2, "notRemoveAllTraitsFromT1T2", false);

            Scribe_Values.Look<bool>(ref androidsCanConsumeLivingPlants, "androidsCanConsumeLivingPlants", true);
            Scribe_Values.Look<bool>(ref hideMenuAllowingForceEatingLivingPlants, "hideMenuAllowingForceEatingLivingPlants", false);
            
            Scribe_Values.Look<bool>(ref notRemoveAllSkillPassionsForBasicAndroids, "notRemoveAllSkillPassionsForBasicAndroids", false);


            Scribe_Values.Look<int>(ref VX3MaxSurrogateControllableAtOnce, "VX3MaxSurrogateControllableAtOnce", 6);

            Scribe_Values.Look<int>(ref defaultSkillT1Animals, "defaultSkillT1Animals", 4);
            Scribe_Values.Look<int>(ref defaultSkillT1Artistic, "defaultSkillT1Artistic", 0);
            Scribe_Values.Look<int>(ref defaultSkillT1Construction, "defaultSkillT1Construction", 4);
            Scribe_Values.Look<int>(ref defaultSkillT1Cooking, "defaultSkillT1Cooking", 4);
            Scribe_Values.Look<int>(ref defaultSkillT1Crafting, "defaultSkillT1Crafting", 4);
            Scribe_Values.Look<int>(ref defaultSkillT1Intellectual, "defaultSkillT1Intellectual", 0);
            Scribe_Values.Look<int>(ref defaultSkillT1Medical, "defaultSkillT1Medical", 4);
            Scribe_Values.Look<int>(ref defaultSkillT1Melee, "defaultSkillT1Melee", 4);
            Scribe_Values.Look<int>(ref defaultSkillT1Mining, "defaultSkillT1Mining", 4);
            Scribe_Values.Look<int>(ref defaultSkillT1Plants, "defaultSkillT1Plants", 4);
            Scribe_Values.Look<int>(ref defaultSkillT1Shoot, "defaultSkillT1Shoot", 4);
            Scribe_Values.Look<int>(ref defaultSkillT1Social, "defaultSkillT1Social", 0);


            Scribe_Values.Look<int>(ref defaultSkillT2Animals, "defaultSkillT2Animals", 6);
            Scribe_Values.Look<int>(ref defaultSkillT2Artistic, "defaultSkillT2Artistic", 0);
            Scribe_Values.Look<int>(ref defaultSkillT2Construction, "defaultSkillT2Construction", 6);
            Scribe_Values.Look<int>(ref defaultSkillT2Cooking, "defaultSkillT2Cooking", 6);
            Scribe_Values.Look<int>(ref defaultSkillT2Crafting, "defaultSkillT2Crafting", 6);
            Scribe_Values.Look<int>(ref defaultSkillT2Intellectual, "defaultSkillT2Intellectual", 0);
            Scribe_Values.Look<int>(ref defaultSkillT2Medical, "defaultSkillT2Medical", 6);
            Scribe_Values.Look<int>(ref defaultSkillT2Melee, "defaultSkillT2Melee", 6);
            Scribe_Values.Look<int>(ref defaultSkillT2Mining, "defaultSkillT2Mining", 6);
            Scribe_Values.Look<int>(ref defaultSkillT2Plants, "defaultSkillT2Plants", 6);
            Scribe_Values.Look<int>(ref defaultSkillT2Shoot, "defaultSkillT2Shoot", 6);
            Scribe_Values.Look<int>(ref defaultSkillT2Social, "defaultSkillT2Social", 0);

            Scribe_Values.Look<int>(ref skillNbpGeneratedOld, "skillNbpGeneratedOld", 5);
            Scribe_Values.Look<int>(ref skillNbpGeneratedBasic, "skillNbpGeneratedOld", 10);
            Scribe_Values.Look<int>(ref skillNbpGeneratedAdvanced, "skillNbpGeneratedOld", 20);

            Scribe_Values.Look<int>(ref hackingNbpGeneratedOld, "hackingNbpGeneratedOld", 5);
            Scribe_Values.Look<int>(ref hackingNbpGeneratedBasic, "hackingNbpGeneratedOld", 10);
            Scribe_Values.Look<int>(ref hackingNbpGeneratedAdvanced, "hackingNbpGeneratedOld", 20);

            Scribe_Values.Look<bool>(ref preventM7T5AppearingInCharacterScreen, "preventM7T5AppearingInCharacterScreen", true);

            Scribe_Values.Look<int>(ref minHoursNaniteFramework, "minHoursNaniteFramework", 8);
            Scribe_Values.Look<int>(ref maxHoursNaniteFramework, "maxHoursNaniteFramework", 48);

            Scribe_Values.Look<int>(ref nbSkillPointsPerSkillT1, "nbSkillPointsPerSkillT1", 150);
            Scribe_Values.Look<int>(ref nbSkillPointsPerSkillT2, "nbSkillPointsPerSkillT2", 250);
            Scribe_Values.Look<int>(ref nbSkillPointsPerSkillT3, "nbSkillPointsPerSkillT3", 600);
            Scribe_Values.Look<int>(ref nbSkillPointsPerSkillT4, "nbSkillPointsPerSkillT4", 1000);
            Scribe_Values.Look<int>(ref nbSkillPointsPerSkillT5, "nbSkillPointsPerSkillT5", 1250);

            Scribe_Values.Look<bool>(ref removeComfortNeedForT3T4, "removeComfortNeedForT3T4", true);

            Scribe_Values.Look<bool>(ref allowHumanDrugsForT3PlusAndroids, "allowHumanDrugsForT3PlusAndroids", true);

            Scribe_Values.Look<int>(ref skyCloudUploadModeForSourceMind, "skyCloudUploadModeForSourceMind", 2);
            Scribe_Values.Look<bool>(ref allowHumanDrugsForAndroids, "allowHumanDrugsForAndroids", false);

            Scribe_Values.Look<bool>(ref removeSimpleMindedTraitOnUpload, "removeSimpleMindedTraitOnUpload", true);

            Scribe_Values.Look<int>(ref minDaysAndroidPaintingCanRust, "minDaysAndroidPaintingCanRust", 15);
            Scribe_Values.Look<int>(ref maxDaysAndroidPaintingCanRust, "maxDaysAndroidPaintingCanRust", 45);
            Scribe_Values.Look<bool>(ref androidsCanRust, "androidsCanRust", true);

            Scribe_Values.Look<float>(ref chanceGeneratedAndroidCanBePaintedOrRust, "chanceGeneratedAndroidCanBePaintedOrRust", 0.45f);

            Scribe_Values.Look<bool>(ref basicAndroidsRandomSKills, "basicAndroidsRandomSKills", false);

            Scribe_Values.Look<bool>(ref androidsAreRare, "androidsAreRare", false);
            Scribe_Values.Look<bool>(ref allowAutoRepaint, "allowAutoRepaint", true);
            Scribe_Values.Look<bool>(ref allowAutoRepaintForPrisoners, "allowAutoRepaintForPrisoners", true);

            Scribe_Values.Look<float>(ref percentageChanceMaleAndroidModel, "percentageChanceMaleAndroidModel", 0.5f);
            

            Scribe_Values.Look<bool>(ref allowT5ToWearClothes, "allowT5ToWearClothes", true);
            Scribe_Values.Look<int>(ref maxAndroidByPortableLWPN, "maxAndroidByPortableLWPN", 5);

            Scribe_Values.Look<int>(ref skillSlotsForAdvancedSkillServers, "skillSlotsForAdvancedSkillServers", 200);
            Scribe_Values.Look<int>(ref skillSlotsForBasicSkillServers, "skillSlotsForBasicSkillServers", 100);
            Scribe_Values.Look<int>(ref skillSlotsForOldSkillServers, "skillSlotsForOldSkillServers", 50);

            Scribe_Values.Look<int>(ref nbSkillPointsPassionT1, "nbSkillPointsPassionT1", 4);
            Scribe_Values.Look<int>(ref nbSkillPointsPassionT2, "nbSkillPointsPassionT2", 5);
            Scribe_Values.Look<int>(ref nbSkillPointsPassionT3, "nbSkillPointsPassionT3", 6);
            Scribe_Values.Look<int>(ref nbSkillPointsPassionT4, "nbSkillPointsPassionT4", 7);
            Scribe_Values.Look<int>(ref nbSkillPointsPassionT5, "nbSkillPointsPassionT5", 8);

            Scribe_Values.Look<bool>(ref keepPuppetBackstory, "keepPuppetBackstory", false);

        }
    }
}