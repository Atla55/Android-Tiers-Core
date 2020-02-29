using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI.Group;
using Verse.AI;
using UnityEngine;
using RimWorld.Planet;
using HarmonyLib;
using System.Reflection;
using Verse.Sound;

namespace MOARANDROIDS
{
    public class GC_ATPP : GameComponent
    {

        public GC_ATPP(Game game)
        {
            this.game = game;
            Utils.GCATPP = this;
            initNull();

            if (!Utils.init)
            {
                try
                {
                    Utils.init = true;

                    try
                    {
                        CPaths.dummyRest = new Need_DummyRest(null);
                    }
                    catch (Exception)
                    {

                    }
                    Utils.CrafterDoctorJob = new List<WorkGiverDef>();

                    Utils.hediffHaveRXChip = DefDatabase<HediffDef>.GetNamed("ATPP_HediffRXChip");
                    Utils.hediffLowNetworkSignal = DefDatabase<HediffDef>.GetNamed("ATPP_LowNetworkSignal");
                    Utils.hediffHaveVX0Chip = DefDatabase<HediffDef>.GetNamed("ATPP_HediffVX0Chip");
                    Utils.hediffHaveVX1Chip = DefDatabase<HediffDef>.GetNamed("ATPP_HediffVX1Chip");
                    Utils.hediffHaveVX2Chip = DefDatabase<HediffDef>.GetNamed("ATPP_HediffVX2Chip");
                    Utils.hediffHaveVX3Chip = DefDatabase<HediffDef>.GetNamed("ATPP_HediffVX3Chip");
                    Utils.hediffRusted = DefDatabase<HediffDef>.GetNamed("ATPP_Rusted");
                    Utils.hediffNoHost = DefDatabase<HediffDef>.GetNamed("ATPP_NoHost");
                    Utils.hediffBlankAndroid = DefDatabase<HediffDef>.GetNamed("ATPP_BlankAndroid");

                    Utils.soundDefSurrogateConnection = DefDatabase<SoundDef>.GetNamed("ATPP_SoundSurrogateConnection");
                    Utils.soundDefSurrogateConnectionStopped = DefDatabase<SoundDef>.GetNamed("ATPP_SoundSurrogateDisconnect");
                    Utils.soundDefTurretConnection = DefDatabase<SoundDef>.GetNamed("ATPP_SoundTurretConnection");
                    Utils.soundDefTurretConnectionStopped = DefDatabase<SoundDef>.GetNamed("ATPP_SoundTurretDisconnect");
                    Utils.soundDefSkyCloudPrimarySystemsOnline = DefDatabase<SoundDef>.GetNamed("ATPP_SoundSkyCloudPrimarySystemsOnline");
                    Utils.soundDefSkyCloudAllMindDisconnected = DefDatabase<SoundDef>.GetNamed("ATPP_SoundSkyCloudAllMindDisconnected");
                    Utils.soundDefSkyCloudMindDeletionCompleted = DefDatabase<SoundDef>.GetNamed("ATPP_SoundSkyCloudMindDeletionCompleted");
                    Utils.soundDefSkyCloudMindMigrationCompleted = DefDatabase<SoundDef>.GetNamed("ATPP_SoundSkyCloudMindMigrationCompleted");
                    Utils.soundDefSkyCloudMindReplicationCompleted = DefDatabase<SoundDef>.GetNamed("ATPP_SoundSkyCloudMindReplicationCompleted");
                    Utils.soundDefSkyCloudPowerFailure = DefDatabase<SoundDef>.GetNamed("ATPP_SoundSkyCloudPowerFailure");
                    Utils.soundDefSkyCloudSkyMindNetworkOffline = DefDatabase<SoundDef>.GetNamed("ATPP_SoundSkyCloudSkyMindNetworkOffline");
                    Utils.soundDefSkyCloudDoorOpened = DefDatabase<SoundDef>.GetNamed("ATPP_SoundSkyCloudDoorOpened");
                    Utils.soundDefSkyCloudDoorClosed = DefDatabase<SoundDef>.GetNamed("ATPP_SoundSkyCloudDoorClosed");
                    Utils.soundDefSkyCloudDeviceActivated = DefDatabase<SoundDef>.GetNamed("ATPP_SoundSkyCloudDeviceActivated");
                    Utils.soundDefSkyCloudDeviceDeactivated = DefDatabase<SoundDef>.GetNamed("ATPP_SoundSkyCloudDeviceDeactivated");
                    Utils.soundDefSkyCloudMindDownloadCompleted = DefDatabase<SoundDef>.GetNamed("ATPP_SoundSkyCloudMindDownloadCompleted");
                    Utils.soundDefSkyCloudMindUploadCompleted = DefDatabase<SoundDef>.GetNamed("ATPP_SoundSkyCloudMindUploadCompleted");
                    Utils.soundDefSkyCloudMindQuarantineMentalState = DefDatabase<SoundDef>.GetNamed("ATPP_SoundSkyCloudMindQuarantineMentalState");

                    Utils.thoughtDefVX0Puppet = DefDatabase<ThoughtDef>.GetNamed("ATPP_VX0PuppetThought");

                    Utils.soundDefSurrogateHacked = DefDatabase<SoundDef>.GetNamed("ATPP_SoundSurrogateHacked");
                    Utils.dummyHeddif = DefDatabase<HediffDef>.GetNamed("ATPP_DummyHediff");
                    Utils.ResearchProjectSkyMindLAN = DefDatabase<ResearchProjectDef>.GetNamed("ATPP_ResearchSkyMindLAN");
                    Utils.ResearchProjectSkyMindWAN = DefDatabase<ResearchProjectDef>.GetNamed("ATPP_ResearchSkyMindWAN");
                    Utils.ResearchAndroidBatteryOverload = DefDatabase<ResearchProjectDef>.GetNamed("ATPP_ResearchBatteryOverload"); 
                    Utils.statDefAndroidTending = DefDatabase<StatDef>.GetNamed("ATPP_AndroidTendQuality");

                    Utils.statDefAndroidSurgerySuccessChance = DefDatabase<StatDef>.GetNamed("AndroidSurgerySuccessChance");

                    Utils.traitSimpleMinded = DefDatabase<TraitDef>.GetNamed("SimpleMindedAndroid",false);

                    //generating list of androids without skin
                    foreach(var el in Utils.ExceptionAndroidList)
                    {
                        if (!Utils.ExceptionAndroidWithSkinList.Contains(el))
                        {
                            Utils.ExceptionAndroidWithoutSkinList.Add(el);
                        }
                    }

                    //Check if TX serie loaded
                    if (DefDatabase<PawnKindDef>.GetNamed("ATPP_AndroidTX2CollectiveSoldier", false) != null)
                    {
                        Utils.TXSERIE_LOADED = true;

                        //Add TX related corpses
                        foreach(var el in Utils.ExceptionTXSerie)
                        {
                            Utils.ExceptionAndroidCorpseList.Add("Corpse_" + el);
                        }
                    }

                    try
                    {
                        Utils.ExceptionRepairableFrameworkHediff = new List<HediffDef> { HediffDefOf.Scratch, HediffDefOf.Bite, HediffDefOf.Burn, HediffDefOf.Cut, HediffDefOf.Gunshot, HediffDefOf.Stab, HediffDefOf.SurgicalCut };

                        HediffDef hd = DefDatabase<HediffDef>.GetNamed("Shredded", false);
                        if (hd != null)
                            Utils.ExceptionRepairableFrameworkHediff.Add(hd);
                        hd = DefDatabase<HediffDef>.GetNamed("Frostbite", false);
                        if (hd != null)
                            Utils.ExceptionRepairableFrameworkHediff.Add(hd);
                        hd = DefDatabase<HediffDef>.GetNamed("Mangled", false);
                        if (hd != null)
                            Utils.ExceptionRepairableFrameworkHediff.Add(hd);
                        hd = DefDatabase<HediffDef>.GetNamed("SurgicalCut", false);
                        if (hd != null)
                            Utils.ExceptionRepairableFrameworkHediff.Add(hd);
                        hd = DefDatabase<HediffDef>.GetNamed("Crush", false);
                        if (hd != null)
                            Utils.ExceptionRepairableFrameworkHediff.Add(hd);
                        hd = DefDatabase<HediffDef>.GetNamed("Crack", false);
                        if (hd != null)
                            Utils.ExceptionRepairableFrameworkHediff.Add(hd);
                    }
                    catch(Exception e)
                    {
                        Log.Message("[ATPP] RepairableFrameworkHediffException " + e.Message + " " + e.StackTrace);
                    }

                    //Apply SolarFlare policy
                    try
                    {
                        Utils.applySolarFlarePolicy();
                    }
                    catch(Exception ex)
                    {
                        Log.Message("[ATPP] applySolarSlarePolicy " + ex.Message + " " + ex.StackTrace);
                    }

                    try
                    {
                        Utils.applyT5ClothesPolicy();
                    }
                    catch(Exception ex)
                    {
                        Log.Message("[ATPP] applyT5ClothesPolicy " + ex.Message + " " + ex.StackTrace);
                    }

                    RecipeDef r = DefDatabase<RecipeDef>.GetNamed("ATPP_DisassembleAndroid", false);
                    //add to disallowedThings all alien race creatures except those provided by this pod ATPP_DisassembleAndroid
                    ThingCategoryDef tcd = DefDatabase<ThingCategoryDef>.GetNamed("alienCorpseCategory", false);

                    if (tcd != null && r != null)
                    {
                        foreach (var el in tcd.childThingDefs)
                        {
                            if (el == null)
                                continue;
                            //Log.Message(el.defName);

                            if (!Utils.ExceptionAndroidCorpseList.Contains(el.defName))
                            {
                                Log.Message("[ATPP] BlacklistigOtherAR  : " + el.defName);
                                r.fixedIngredientFilter.SetAllow(el, false);
                            }
                        }
                    }

                    //Dynamicaly add "Meat_Human" to recipe TX3/TX4
                    if (Utils.TXSERIE_LOADED)
                    {
                        try
                        {
                            ThingDef tx4 = DefDatabase<ThingDef>.GetNamed("ATPP_Android4TX", false);
                            ThingDef tx3 = DefDatabase<ThingDef>.GetNamed("ATPP_Android3TX", false);

                            ThingDef humanMeat = DefDatabase<ThingDef>.GetNamed("Meat_Human", false);
                            if (tx3 != null && tx4 != null && humanMeat != null)
                            {
                                tx4.butcherProducts.Add(new ThingDefCountClass(humanMeat, 100));
                                tx3.butcherProducts.Add(new ThingDefCountClass(humanMeat, 100));
                            }

                        }
                        catch (Exception e)
                        {
                            Log.Message("[ATPP] GC_ATPP.HumanMeatInjection " + e.Message + " " + e.StackTrace);
                        }
                    }

                    Utils.ExceptionAndroidCanReloadWithPowerList = Utils.ExceptionAndroidList.Concat(Utils.ExceptionAndroidAnimalPowered).ToList();

                    //RunTime patching
                    foreach (var td in DefDatabase<ThingDef>.AllDefsListForReading)
                    {
                        try
                        {
                            if (td != null && td.race != null)
                            {
                                //Dynamic fix Fluffy_BirdsAndBees mod which add sex to androids
                                if (Utils.BIRDSANDBEES_LOADED && (Utils.ExceptionAndroidList.Contains(td.defName) || Utils.ExceptionAndroidAnimals.Contains(td.defName)))
                                {
                                    Log.Message("[ATPP] BIRDSANDBEES.fix " + td.defName);
                                    try
                                    {
                                        //remove neutering operation
                                        if (td.recipes != null)
                                        {
                                            foreach (var cr in td.recipes.ToList())
                                            {
                                                if (cr.defName == "Neuter" || cr.defName == "InstallBasicReproductiveOrgans" || cr.defName == "InstallBionicReproductiveOrgans")
                                                {
                                                    td.recipes.Remove(cr);
                                                }
                                            }
                                        }

                                        //Cut reproductive organs
                                        if (td.race.body != null && td.race.body.corePart != null && td.race.body.corePart.parts != null)
                                        {
                                            foreach (var cbp in td.race.body.corePart.parts.ToList())
                                            {
                                                if (cbp.def.defName == "ReproductiveOrgans")
                                                {
                                                    td.race.body.corePart.parts.Remove(cbp);
                                                }
                                            }
                                            foreach (var cbp in td.race.body.AllParts.ToList())
                                            {
                                                if (cbp.def.defName == "ReproductiveOrgans")
                                                {
                                                    td.race.body.AllParts.Remove(cbp);
                                                }
                                            }
                                        }

                                        //Remove old-age  hediffgivers
                                        if (td.race.hediffGiverSets != null)
                                        {
                                            foreach (var hg in td.race.hediffGiverSets.ToList())
                                            {
                                                if (hg.defName == "HumanoidFertility")
                                                    td.race.hediffGiverSets.Remove(hg);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Message("[ATPP] BIRDSANDBEES.fix " + ex.Message + " " + ex.StackTrace);
                                    }
                                }

                                if (td.race.intelligence == Intelligence.Humanlike)
                                {

                                    //SkyMind
                                    CompProperties cp = new CompProperties();
                                    cp.compClass = typeof(CompSkyMind);
                                    td.comps.Add(cp);

                                    //CompSurrogate
                                    if (td.defName != "M7Mech")
                                    {
                                        cp = new CompProperties();
                                        cp.compClass = typeof(CompSurrogateOwner);
                                        td.comps.Add(cp);
                                    }

                                    cp = new CompProperties();
                                    cp.compClass = typeof(CompAndroidState);
                                    td.comps.Add(cp);

                                    //Si androide on va venir stocké dans la Raceprops s'il sagit d'un androide evolué ou non
                                    if (Utils.ExceptionAndroidList.Contains(td.defName))
                                    {
                                        if (Utils.ExceptionAndroidListAdvanced.Contains(td.defName))
                                            td.race.gestationPeriodDays = 2;
                                        else
                                            td.race.gestationPeriodDays = 1;

                                        try
                                        {
                                            if (!Settings.allowHumanDrugsForAndroids)
                                            {
                                                //Remove autogenerated administerXXX for androids
                                                foreach (var el in td.AllRecipes.ToList())
                                                {
                                                    foreach (var blacklistedFood in Utils.BlacklistAndroidFood)
                                                    {
                                                        if (el.defName == "Administer_" + blacklistedFood)
                                                        {
                                                            td.AllRecipes.Remove(el);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Log.Message("[ATPP] RemovingAndroidAdministerFood " + e.Message + " " + e.StackTrace);
                                        }
                                    }
                                }
                                else if (Utils.ExceptionAndroidAnimalPowered.Contains(td.defName))
                                {
                                    //Log.Message("=>" + td.defName);
                                    CompProperties cp = new CompProperties();
                                    cp = new CompProperties();
                                    cp.compClass = typeof(CompAndroidState);
                                    td.comps.Add(cp);
                                }
                            }
                            else
                            {
                                if (Utils.ExceptionAutodoors.Contains(td.defName))
                                {
                                    CompProperties cp = new CompProperties();
                                    cp.compClass = typeof(CompAutoDoor);
                                    td.comps.Add(cp);

                                    //SkyMind
                                    cp = new CompProperties();
                                    cp.compClass = typeof(CompSkyMind);
                                    td.comps.Add(cp);
                                }
                                else if(td.defName == "ATPP_AndroidPod" || td.defName == "ATPP_AndroidPodMech" || td.defName == "AndroidOperationBed")
                                {
                                    CompProperties cp = new CompProperties();
                                    cp.compClass = typeof(CompAndroidPod);
                                    td.comps.Add(cp);

                                    td.tickerType = TickerType.Normal;

                                    if(td.defName == "AndroidOperationBed")
                                    {
                                        CompProperties_Power cp2 = new CompProperties_Power();
                                        cp2.compClass = typeof(CompPowerTrader);
                                        cp2.shortCircuitInRain = true;
                                        cp2.basePowerConsumption = 80;
                                        td.comps.Add(cp2);
                                    }
                                }
                                else if (td.thingClass != null && ( td.thingClass == typeof(Building_Turret) || td.thingClass.IsSubclassOf(typeof(Building_Turret))))
                                {
                                    //SkyMind
                                    CompProperties cp = new CompProperties();
                                    cp.compClass = typeof(CompSkyMind);
                                    td.comps.Add(cp);

                                    //RemoteTurret
                                    cp = new CompProperties();
                                    cp.compClass = typeof(CompRemotelyControlledTurret);
                                    td.comps.Add(cp);
                                }
                                else
                                {
                                    if (Utils.ExceptionSkyCloudCores.Contains(td.defName))
                                        continue;

                                    if (td.comps != null)
                                    {
                                        bool found = false;
                                        bool flickable = false;

                                        foreach (var e in td.comps)
                                        {
                                            if (e.compClass == null)
                                                continue;

                                            if (e.compClass == typeof(CompFlickable))
                                                flickable = true;

                                            if (e.compClass == typeof(CompPowerTrader) || (e.compClass == typeof(CompPowerPlant) || e.compClass.IsSubclassOf(typeof(CompPowerPlant))))
                                            {
                                                found = true;
                                            }
                                        }

                                        if (found && flickable)
                                        {
                                            CompProperties cp = new CompProperties();
                                            cp.compClass = typeof(CompSkyMind);
                                            td.comps.Add(cp);
                                        }

                                    }
                                }
                            }
                        }
                        catch(Exception e)
                        {
                            Log.Message("[ATPP] Runtime.Patching.Comps "+e.Message+" "+e.StackTrace);
                        }
                    }

                    Utils.M7Mech = DefDatabase<ThingDef>.GetNamed("M7Mech", false);

                    try
                    {
                        Utils.M7Mech.inspectorTabsResolved.Add(InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Needs)));
                    }
                    catch (Exception)
                    {

                    }

                    //Patching M7 ThinkNode
                    ThinkTreeDef ttd = DefDatabase<ThinkTreeDef>.GetNamed("MechM7Like", false);

                    try
                    {
                        if (ttd != null)
                        {
                            ThinkNode tn = new ThinkNode_ConditionalMustKeepLyingDownM7Surrogate();
                            tn.subNodes.Add(new JobGiver_KeepLyingDown());

                            ttd.thinkRoot.subNodes.Insert(0, tn);

                            ThinkNode tnt = new ThinkNode_ConditionalM7Charging();

                            tnt.subNodes.Add(new JobGiver_GetFood());

                            //Tentative de trouver emplacement ou ajouter le batterieRecharcheWork (juste aprés le ThinkNode_ConditionalColonist)
                            int index = 0;
                            bool found = false;
                            foreach(var el in ttd.thinkRoot.subNodes)
                            {
                                if(el is ThinkNode_ConditionalColonist)
                                {
                                    found = true;
                                    index--;
                                    break;
                                }
                                index++;
                            }

                            if (index < 0)
                                index = 1;

                            if (!found)
                            {
                                index = 1;
                            }

                            ttd.thinkRoot.subNodes.Insert(index, tnt);

                            if (Utils.SEARCHANDDESTROY_LOADED)
                            {
                                var type = Utils.searchAndDestroyAssembly.GetType("SearchAndDestroy.ThinkNode_ConditionalSearchAndDestroy");
                                ThinkNode sad = (ThinkNode)Activator.CreateInstance(type);
                                ThinkNode_Priority tp = new ThinkNode_Priority();
                                JobGiver_AIFightEnemies jgFE = new JobGiver_AIFightEnemies();
                                Traverse trJgFE = Traverse.Create(jgFE);
                                trJgFE.Field("targetKeepRadius").SetValue(72);
                                trJgFE.Field("targetAcquireRadius").SetValue(200);

                                tp.subNodes.Add(jgFE);
                                tp.subNodes.Add(new JobGiver_AIGotoNearestHostile());

                                sad.subNodes.Add(tp);

                                ttd.thinkRoot.subNodes.Insert(index+1, sad);
                            }
                        }
                        else
                        {
                            Log.Message("[ATPP] MechM7 ThinkTree not found");
                        }
                    }
                    catch(Exception e)
                    {
                        Log.Message("[ATPP] MechM7 ThinkTree patching issue "+e.Message+" "+e.StackTrace);
                    }


                    //Dynamic SMartMedecine patching
                    if (Utils.SMARTMEDICINE_LOADED && Utils.smartMedicineAssembly != null)
                    {
                        try
                        {
                            //Utils.harmonyInstance;
                            var original = Utils.smartMedicineAssembly.GetType("SmartMedicine.FindBestMedicine").GetMethod("Find", BindingFlags.Static | BindingFlags.Public);
                            var prefix = typeof(Utils).GetMethod("FindBestMedicinePrefix", BindingFlags.Static | BindingFlags.Public);
                            var postfix = typeof(Utils).GetMethod("FindBestMedicinePostfix", BindingFlags.Static | BindingFlags.Public);
                            Utils.harmonyInstance.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
                        }
                        catch(Exception e)
                        {
                            Log.Message("[ATPP] SmartMedicinePatch " + e.Message + " " + e.StackTrace);
                        }
                    }

                    //Dynamic MedicinePatch patching
                    if(Utils.MEDICINEPATCH_LOADED && Utils.medicinePatchAssembly != null) {
                        try
                        {
                            //Utils.harmonyInstance;
                            var original = Utils.medicinePatchAssembly.GetType("ModMedicinePatch.ModMedicinePatch").GetMethod("DynamicMedicalCareSetter", BindingFlags.Static | BindingFlags.Public);
                            var prefix = typeof(Utils).GetMethod("DynamicMedicalCareSetterPrefix", BindingFlags.Static | BindingFlags.Public);
                            Utils.harmonyInstance.Patch(original, new HarmonyMethod(prefix));

                            original = Utils.medicinePatchAssembly.GetType("ModMedicinePatch.MedicalCareSetter").GetMethod("_Postfix", BindingFlags.Static | BindingFlags.Public);
                            prefix = typeof(Utils).GetMethod("DynamicMedicalCareSetterPrefixPostfix", BindingFlags.Static | BindingFlags.Public);
                            Utils.harmonyInstance.Patch(original,new HarmonyMethod(prefix));
                        }
                        catch (Exception e)
                        {
                            Log.Message("[ATPP] MedicinePatchPatching " + e.Message + " " + e.StackTrace);
                        }
                    }

                    //PRISON LABOR Patching
                    if (Utils.PRISONLABOR_LOADED)
                    {
                        try
                        {
                            MethodInfo postfix = null;
                            MethodInfo original =null;
                            MethodInfo prefix = null;

                            //Utils.harmonyInstance;
                            Type t1 = Utils.prisonLaborAssembly.GetType("PrisonLabor.Core.PrisonLaborUtility");

                            //Try for old release
                            if (t1 == null)
                            {
                                Log.Message("[ATPP] PrisonLabor V1 not detected trying add compatibility with old release");

                                original = Utils.prisonLaborAssembly.GetType("PrisonLabor.PrisonLaborUtility").GetMethod("WorkTime", BindingFlags.Static | BindingFlags.Public);
                                prefix = typeof(CPaths).GetMethod("PrisonLabor_WorkTimePrefix", BindingFlags.Static | BindingFlags.Public);
                                Utils.harmonyInstance.Patch(original, new HarmonyMethod(prefix));

                                original = Utils.prisonLaborAssembly.GetType("PrisonLabor.Need_Motivation").GetMethod("get_LazinessRate", BindingFlags.Instance | BindingFlags.NonPublic);
                                prefix = typeof(CPaths).GetMethod("PrisonLabor_GetChangePointsPrefix", BindingFlags.Static | BindingFlags.Public);
                                postfix = typeof(CPaths).GetMethod("PrisonLabor_GetChangePointsPostfix", BindingFlags.Static | BindingFlags.Public);
                            }
                            else {
                                original = t1.GetMethod("WorkTime", BindingFlags.Static | BindingFlags.Public);
                                prefix = typeof(CPaths).GetMethod("PrisonLabor_WorkTimePrefix", BindingFlags.Static | BindingFlags.Public);
                                Utils.harmonyInstance.Patch(original, new HarmonyMethod(prefix));

                                original = Utils.prisonLaborAssembly.GetType("PrisonLabor.Core.Needs.Need_Motivation").GetMethod("GetChangePoints", BindingFlags.Instance | BindingFlags.NonPublic);
                                prefix = typeof(CPaths).GetMethod("PrisonLabor_GetChangePointsPrefix", BindingFlags.Static | BindingFlags.Public);
                                postfix = typeof(CPaths).GetMethod("PrisonLabor_GetChangePointsPostfix", BindingFlags.Static | BindingFlags.Public);
                            }

                            Utils.harmonyInstance.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
                        }
                        catch (Exception e)
                        {
                            Log.Message("[ATPP] PrisonLaborPatching " + e.Message + " " + e.StackTrace);
                        }
                    }

                    //SoS2 patching
                    if (Utils.SAVEOURSHIP2_LOADED)
                    {
                        try
                        {
                            //Utils.harmonyInstance;
                            var original = Utils.saveOurShip2Assembly.GetType("SaveOurShip2.ShipInteriorMod2").GetMethod("hasSpaceSuit", BindingFlags.Static | BindingFlags.Public);
                            var postfix = typeof(CPaths).GetMethod("SaveOurShip2_hasSpaceSuit", BindingFlags.Static | BindingFlags.Public);
                            Utils.harmonyInstance.Patch(original, null, new HarmonyMethod(postfix));
                        }
                        catch (Exception e)
                        {
                            Log.Message("[ATPP] SaveOurShip2Patching " + e.Message + " " + e.StackTrace);
                        }
                    }

                    if (Utils.HOSPITALITY_LOADED)
                    {
                        try
                        {
                            //Utils.harmonyInstance;
                            var original = Utils.hospitalityAssembly.GetType("Hospitality.BedUtility").GetMethod("FindBedFor", BindingFlags.Static | BindingFlags.Public);
                            var prefix = typeof(CPaths).GetMethod("Hopistality_FindBedForPrefix", BindingFlags.Static | BindingFlags.Public);
                            var postfix = typeof(CPaths).GetMethod("Hopistality_FindBedForPostfix", BindingFlags.Static | BindingFlags.Public);
                            Utils.harmonyInstance.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
                        }
                        catch (Exception e)
                        {
                            Log.Message("[ATPP] HospitalityPatching " + e.Message + " " + e.StackTrace);
                        }
                    }

                    if (Utils.POWERPP_LOADED)
                    {
                        try
                        {
                            var original = Utils.powerppAssembly.GetType("aRandomKiwi.PPP.CompLocalWirelessPowerEmitter").GetMethod("CompInspectStringExtra", BindingFlags.Instance | BindingFlags.Public);
                            var postfix = typeof(CPaths).GetMethod("PowerPP_CompLocalWirelessPowerEmitter_CompInspectStringExtra", BindingFlags.Static | BindingFlags.Public);
                            Utils.harmonyInstance.Patch(original, null, new HarmonyMethod(postfix));
                            //PowerPP_CompLocalWirelessPowerEmitter_CompInspectStringExtra
                        }
                        catch (Exception e)
                        {
                            Log.Message("[ATPP] Power++Patching " + e.Message + " " + e.StackTrace);
                        }
                    }

                    if (Utils.QEE_LOADED)
                    {
                        try
                        {
                            var original = Utils.qeeAssembly.GetType("QEthics.Building_PawnVatGrower").GetMethod("GetGizmos", BindingFlags.Instance | BindingFlags.Public);
                            var postfix = typeof(CPaths).GetMethod("QEE_BuildingPawnVatGrower_GetGizmosPostfix", BindingFlags.Static | BindingFlags.Public);
                            Utils.harmonyInstance.Patch(original, null, new HarmonyMethod(postfix));

                            original = Utils.qeeAssembly.GetType("QEthics.Building_PawnVatGrower").GetMethod("TryMakeClone", BindingFlags.Instance | BindingFlags.Public);
                            var prefix = typeof(CPaths).GetMethod("QEE_BuildingPawnVatGrower_TryMakeClonePrefix", BindingFlags.Static | BindingFlags.Public);
                            postfix = typeof(CPaths).GetMethod("QEE_BuildingPawnVatGrower_TryMakeClonePostfix", BindingFlags.Static | BindingFlags.Public); 
                            Utils.harmonyInstance.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
                            //
                            //QEE_BUildingPawnVatGrower_TryExtractProductPrefix

                            original = Utils.qeeAssembly.GetType("QEthics.Building_PawnVatGrower").GetMethod("TryExtractProduct", BindingFlags.Instance | BindingFlags.Public);
                            prefix = typeof(CPaths).GetMethod("QEE_BUildingPawnVatGrower_TryExtractProductPrefix", BindingFlags.Static | BindingFlags.Public);
                            postfix = typeof(CPaths).GetMethod("QEE_BUildingPawnVatGrower_TryExtractProductPostfix", BindingFlags.Static | BindingFlags.Public);
                            Utils.harmonyInstance.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));

                            original = Utils.qeeAssembly.GetType("QEthics.Building_GrowerBase").GetMethod("get_CraftingProgressPercent", BindingFlags.Instance | BindingFlags.Public);
                            postfix = typeof(CPaths).GetMethod("QEE_Building_GrowerBase_get_CraftingProgressPercentPostfix", BindingFlags.Static | BindingFlags.Public);
                            Utils.harmonyInstance.Patch(original, null, new HarmonyMethod(postfix));
                            
                        }
                        catch(Exception e)
                        {
                            Log.Message("[ATPP] QEE Patching " + e.Message + " " + e.StackTrace);
                        }
                    }


                    try
                    {
                        var originalVanilla = typeof(PawnApparelGenerator).GetNestedType("PossibleApparelSet", BindingFlags.NonPublic | BindingFlags.Instance).GetMethod("PairOverlapsAnything", BindingFlags.Public | BindingFlags.Instance);
                        var postfixVanilla = typeof(CPaths).GetMethod("RimworldVanilla_PawnApparelGeneratorPossibleApparelSetPairOverlapsAnything", BindingFlags.Static | BindingFlags.Public);

                        Utils.harmonyInstance.Patch(originalVanilla, null, new HarmonyMethod(postfixVanilla));
                    }
                    catch(Exception e)
                    {
                        Log.Message("[ATPP] PawnApparelGeneratorPatching " + e.Message + " " + e.StackTrace);
                    }

                    //Fluffy WorkTab patching
                    /*if (Utils.WORKTAB_LOADED)
                    {
                        try
                        {
                            //Utils.harmonyInstance;
                            var original = Utils.workTabAssembly.GetType("WorkTab.Pawn_WorkSettings_CacheWorkGiversInOrder").GetMethod("Prefix", BindingFlags.Static | BindingFlags.Public);
                            var postfix = typeof(CPaths).GetMethod("WorkTab_PrefixPostFix", BindingFlags.Static | BindingFlags.Public);
                            Utils.harmonyInstance.Patch(original, null, new HarmonyMethod(postfix));
                        }
                        catch (Exception e)
                        {
                            Log.Message("[ATPP] WorkTabPatching " + e.Message + " " + e.StackTrace);
                        }
                    }*/


                    //Application parametre d'alimentation des plantes vivantes des androides
                    Utils.applyLivingPlantPolicy();

                    //Genration WorkType custom de crafter soigner d'androides
                    WorkTypeDef doctor = DefDatabase<WorkTypeDef>.GetNamed("Doctor",false);
                    Utils.WorkTypeDefSmithing = DefDatabase<WorkTypeDef>.GetNamed("Smithing",false);

                    //Duplication doctor WorkGiverDef

                    int nbJobDrC = 0;
                    foreach (var e in doctor.workGiversByPriority)
                    {
                        try
                        {
                            WorkGiverDef cd = new WorkGiverDef();
                            cd.workType = Utils.WorkTypeDefSmithing;
                            cd.priorityInType = e.priorityInType;
                            cd.verb = e.verb;
                            cd.gerund = e.gerund;
                            cd.requiredCapacities = e.requiredCapacities;
                            cd.label = e.label;
                            cd.giverClass = e.giverClass;
                            cd.billGiversAllAnimals = e.billGiversAllAnimals;
                            cd.billGiversAllAnimalsCorpses = e.billGiversAllAnimalsCorpses;
                            cd.billGiversAllHumanlikes = e.billGiversAllHumanlikes;
                            cd.billGiversAllHumanlikesCorpses = e.billGiversAllHumanlikesCorpses;
                            cd.billGiversAllMechanoidsCorpses = e.billGiversAllMechanoidsCorpses;
                            cd.canBeDoneWhileDrafted = e.canBeDoneWhileDrafted;
                            cd.tagToGive = e.tagToGive;
                            cd.scanThings = e.scanThings;
                            cd.scanCells = e.scanCells;
                            cd.workTags = e.workTags;
                            cd.autoTakeablePriorityDrafted = e.autoTakeablePriorityDrafted;
                            cd.feedAnimalsOnly = e.feedAnimalsOnly;
                            cd.feedHumanlikesOnly = e.feedHumanlikesOnly;
                            cd.fixedBillGiverDefs = e.fixedBillGiverDefs;
                            cd.emergency = e.emergency;
                            cd.gerund = e.gerund;
                            cd.verb = e.verb;
                            cd.label = e.label;

                            cd.defName = "ATPP_CrafterHealer" + e.defName;

                            Utils.CrafterDoctorJob.Add(cd);
                            nbJobDrC++;
                        }
                        catch (Exception ex)
                        {
                            Log.Message("[ATPP] Duplication.DoctorWorkGiverDefs " + ex.Message + " " + ex.StackTrace);
                        }
                    }

                    Log.Message("[ATPP] " + nbJobDrC + " Care job collected for crafting injection");

                    try
                    {
                        //Ajout a la liste des def des defs synthetiques pour que si d'autre mods mess avec les workGiversByPriority il n'y est pas de pb
                        foreach (var el in Utils.CrafterDoctorJob)
                        {
                            DefDatabase<WorkGiverDef>.Add(el);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Message("[ATPP] Duplication.DoctorWorkGiverDefs.AddWGD " + ex.Message + " " + ex.StackTrace);
                    }

                    //Ajout des jobs de soins au jobs de crafting ==> ne seront realisable que grace aux patch checkant que le patient est un android et le docteur a l'option doc activée !!!
                    Utils.WorkTypeDefSmithing.workGiversByPriority = Utils.CrafterDoctorJob.Concat(Utils.WorkTypeDefSmithing.workGiversByPriority).ToList();

                    try
                    {
                        //Ajout PawnkindDefs des androids
                        List<string> pkd = new List<string> { "AndroidT1RaiderFactionSpecific", "AndroidT2RaiderFactionSpecific", "AndroidT3RaiderFactionSpecific", "AndroidT4RaiderFactionSpecific" };
                        foreach (var x in pkd)
                        {
                            PawnKindDef p = DefDatabase<PawnKindDef>.GetNamed(x,false);
                            Utils.AndroidsPKDHostile.Add(p);
                        }
                        pkd = new List<string> { "ATPP_AndroidTX2RaiderFactionSpecific", "ATPP_AndroidTX2KRaiderFactionSpecific", "ATPP_AndroidTX3RaiderFactionSpecific", "ATPP_AndroidTX4RaiderFactionSpecific" };
                        foreach (var x in pkd)
                        {
                            PawnKindDef p = DefDatabase<PawnKindDef>.GetNamed(x,false);
                            Utils.AndroidsXSeriePKDHostile.Add(p);
                        }
                        pkd = new List<string> { "ATPP_AndroidTX2IRaiderFactionSpecific", "ATPP_AndroidTX2KIRaiderFactionSpecific", "ATPP_AndroidTX3IRaiderFactionSpecific", "ATPP_AndroidTX4IRaiderFactionSpecific" };
                        foreach (var x in pkd)
                        {
                            PawnKindDef p = DefDatabase<PawnKindDef>.GetNamed(x,false);
                            Utils.AndroidsXISeriePKDHostile.Add(p);
                        }



                        pkd = new List<string> { "AndroidT1CollectiveSoldier", "AndroidT2CollectiveSoldier", "AndroidT3CollectiveSoldier", "AndroidT4CollectiveSoldier" };
                        foreach (var x in pkd)
                        {
                            PawnKindDef p = DefDatabase<PawnKindDef>.GetNamed(x,false);
                            Utils.AndroidsPKDNeutral.Add(p);
                        }
                        pkd = new List<string> { "ATPP_AndroidTX2CollectiveSoldier", "ATPP_AndroidTX2KCollectiveSoldier", "ATPP_AndroidTX3CollectiveSoldier", "ATPP_AndroidTX4CollectiveSoldier" };
                        foreach (var x in pkd)
                        {
                            PawnKindDef p = DefDatabase<PawnKindDef>.GetNamed(x,false);
                            Utils.AndroidsXSeriePKDNeutral.Add(p);
                        }
                        pkd = new List<string> { "ATPP_AndroidTX2ICollectiveSoldier", "ATPP_AndroidTX2KICollectiveSoldier", "ATPP_AndroidTX3ICollectiveSoldier", "ATPP_AndroidTX4ICollectiveSoldier" };
                        foreach (var x in pkd)
                        {
                            PawnKindDef p = DefDatabase<PawnKindDef>.GetNamed(x,false);
                            Utils.AndroidsXISeriePKDNeutral.Add(p);
                        }
                    }
                    catch(Exception ex)
                    {
                        Log.Message("[ATPP] PawnKindDefGathering " + ex.Message + " " + ex.StackTrace);
                    }


                    //Remplissage des mentalBreakDef des virused lite
                    List<string> selMentalBreaks = new List<string> { "Wander_Sad", "InsultingSpree", "TargetedInsultingSpree", "MurderousRage" };
                    MentalBreakDef mb;

                    foreach (var ct in selMentalBreaks)
                    {
                        mb = DefDatabase<MentalBreakDef>.GetNamed(ct, false);
                        if (mb != null)
                            Utils.VirusedRandomMentalBreak.Add(mb);
                    }

                    Log.Message("[ATPP] "+selMentalBreaks.Count+" MentalBreaks collected");

                    //Adding bad traits
                    TraitDef t;
                    List<string> selTraits = new List<string> { "Pyromaniac", "Wimp", "CreepyBreathing", "AnnoyingVoice", "Jealous", "NightOwl", "Brawler", "Nudist", "Gourmand" };

                    foreach (var st in selTraits) {
                        t = DefDatabase<TraitDef>.GetNamed(st, false);
                        if (t != null)
                            Utils.RansomAddedBadTraits.Add(t);
                    }

                    //Si presence HellUnit ajout schema creation surrogate, sinon suppression recipedefs
                    RecipeDef recipHU = DefDatabase<RecipeDef>.GetNamed("ATPP_CreateHellDrone");
                    ThingDef tdHU = DefDatabase<ThingDef>.GetNamed("ATPP_SHUSurrogateGeneratorAI");
                    
                    if (Utils.HELLUNIT_LOADED)
                    {
                        ((CompProperties_SurrogateSpawner)tdHU.comps.First()).Pawnkind = DefDatabase<PawnKindDef>.GetNamed("AndroidHellUnit");
                    }
                    else
                    {
                        DefDatabase<RecipeDef>.AllDefsListForReading.Remove(recipHU);
                        DefDatabase<ThingDef>.AllDefsListForReading.Remove(tdHU);
                    }
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] GC_ATPP.CTOR(Init)  Fatal Error : " + e.Message+" - "+e.StackTrace);
                }
            }
            else
            {
                Utils.lastDoorOpenedVocalGT = 0;
                Utils.lastDoorClosedVocalGT = 0;
                Utils.lastDeviceActivatedVocalGT = 0;
                Utils.lastDeviceDeactivatedVocalGT = 0;
                Utils.lastPlayedVocalWarningNoSkyMindNetGT = 0;
            }
        }

        public override void LoadedGame()
        {
            base.LoadedGame();

            //Reconnection des surrogates en caravane
            reconnectSurrogatesInCaravans();
            removeBlacklistedAndroidsHediffs();
            checkRemoveAndroidFactions();
        }


        private void removeBlacklistedAndroidsHediffs()
        {
            List<Hediff> toDel = new List<Hediff>();

            foreach(var map in Find.Maps)
            {
                foreach(var p in map.mapPawns.AllPawns)
                {
                    if (p.IsAndroidTier() && p.health != null && p.health.hediffSet != null)
                    {
                        foreach(var he in p.health.hediffSet.hediffs)
                        {
                            if (Utils.BlacklistAndroidHediff.Contains(he.def.defName))
                                toDel.Add(he);
                        }

                        if(toDel.Count() > 0)
                        {
                            foreach(var h in toDel)
                            {
                                p.health.hediffSet.hediffs.Remove(h);
                            }
                            toDel.Clear();
                        }
                    }

                }
            }
        }

        private void reconnectSurrogatesInCaravans()
        {
            foreach (var c in Find.World.worldObjects.Caravans)
            {
                foreach (var p in c.pawns)
                {
                    if (p.IsSurrogateAndroid())
                    {
                        CompAndroidState cas = p.TryGetComp<CompAndroidState>();
                        if(cas != null && cas.surrogateController != null)
                        {
                            connectUser(p);
                        }
                    }
                }
            }
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            initNull();
            reset();
            checkRemoveAndroidFactions();
        }

        public override void ExposeData()
        {
            base.ExposeData();

            //Initialisation des champs null le cas echeant Et si param ok suppresion des relations de bonding existantes
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                reset();
            }

            Scribe_Values.Look<int>(ref this.SkyCloureCoreID, "ATPP_SkyCloureCoreID", 1);
            Scribe_Values.Look<int>(ref this.S0NID, "ATPP_S0NID", 1);
            Scribe_Values.Look<int>(ref this.S1NID, "ATPP_S1NID", 1);
            Scribe_Values.Look<int>(ref this.S2NID, "ATPP_S2NID", 1);
            Scribe_Values.Look<int>(ref this.S3NID, "ATPP_S3NID", 1);
            Scribe_Values.Look<int>(ref this.S4NID, "ATPP_S4NID", 1);
            Scribe_Values.Look<int>(ref this.SX2NID, "ATPP_SX2NID", 1);
            Scribe_Values.Look<int>(ref this.SX2KNID, "ATPP_SX2KNID", 1);
            Scribe_Values.Look<int>(ref this.SX3NID, "ATPP_SX3NID", 1);
            Scribe_Values.Look<int>(ref this.SX4NID, "ATPP_SX4NID", 1);
            Scribe_Values.Look<int>(ref this.S10NID, "ATPP_S10NID", 1);
            Scribe_Values.Look<int>(ref this.nbSlot, "nbSlot", 0);
            Scribe_Values.Look<int>(ref this.nbSecuritySlot, "nbSecuritySlot", 0);
            Scribe_Values.Look<int>(ref this.nbSkillSlot, "nbSkillSlot", 0);
            Scribe_Values.Look<int>(ref this.nbHackingPoints, "ATPP_nbHackingPoints", 0);
            Scribe_Values.Look<int>(ref this.nbSkillPoints, "ATPP_nbSkillPoints", 0);

            Scribe_Collections.Look(ref QEEAndroidHairColor, "ATPP_QEEAndroidHairColor", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref QEESkinColor, "ATPP_QEESkinColor", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref QEEAndroidHair, "ATPP_QEEAndroidHair", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref VatGrowerLastPawnIsTX, "ATPP_VatGrowerLastPawnIsTX", LookMode.Value, LookMode.Value);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                initNull();
            }
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            int CGT = Find.TickManager.TicksGame;

            //TOutes les 1 sec
            if(CGT % 60 == 0)
            {
                if (!appliedSettingsOnReload)
                {
                    applyLowSkyMindNetworkSettings();
                    appliedSettingsOnReload = true;
                }
                checkVirusedThings();

            }

            //Toutes les 6 sec check etat réseau
            if(CGT % 360 == 0)
            {
                if(!Settings.disableLowNetworkMalus)
                    checkSkyMindSignalPerformance();

                checkSkyMindAutoReconnect();

                //Check solarFlare dans les caravans
                checkSolarFlarStuffInCaravans();

                if (Utils.POWERPP_LOADED)
                    checkDisconnectedFromLWPNAndroid();
            }
        }

        /*
         * Vire les androids ne pouvant plus être alimentés
         */
        public void checkDisconnectedFromLWPNAndroid()
        {
            foreach(var el in listerLWPNAndroid)
            {
                float availablePower = Utils.getCurrentAvailableEnergy(el.Key.PowerComp.PowerNet);
                if (el.Value.Count == 0)
                    continue;

                int nbConn = 0;
                foreach (var android in el.Value.ToList())
                {
                    CompAndroidState cas = android.TryGetComp<CompAndroidState>();
                    if (cas != null && !cas.useBattery)
                    {
                        el.Value.Remove(android);
                        continue;
                    }

                    bool nonFunctionalLWPN = el.Key.Destroyed || el.Key.IsBrokenDown() || !el.Key.TryGetComp<CompPowerTrader>().PowerOn;

                    //Déduction qt consommé par android
                    int qtConsummed = Utils.getConsumedPowerByAndroid(android.def.defName);
                    if (nonFunctionalLWPN || (availablePower - qtConsummed < 0) || (el.Key.def.defName != "ARKPPP_LocalWirelessPowerEmitter" && nbConn >= Settings.maxAndroidByPortableLWPN))
                    {
                        el.Value.Remove(android);
                        el.Key.TryGetComp<CompPowerTrader>().PowerOutput += qtConsummed;
                        if (cas != null)
                            cas.connectedLWPNActive = false;
                    }
                    else
                    {
                        availablePower -= qtConsummed;
                        //Incrémentation de la batterie le cas echeant (et animation de chargement de batterie)
                        android.needs.food.CurLevelPercentage += Settings.percentageOfBatteryChargedEach6Sec;
                        if(android.needs.food.CurLevelPercentage <= 0.95f)
                            Utils.throwChargingMote(android);

                        nbConn++;
                    }
                }
            }
        }

        public void checkSolarFlarStuffInCaravans()
        {
            if (Find.WorldObjects == null)
                return; 

            foreach(var c in Find.WorldObjects.Caravans)
            {
                foreach(var p in c.pawns)
                {
                    CompAndroidState cas = p.TryGetComp<CompAndroidState>();
                    if(cas != null)
                    {
                        cas.checkSolarFlareStuff();
                    }
                }
            }
        }


        public void applyLowSkyMindNetworkSettings()
        {
            if (Settings.disableLowNetworkMalus)
                Utils.removeAllSlowNetworkHediff();
            else
            {
                if (Settings.disableLowNetworkMalusInCaravans)
                    Utils.removeAllSlowNetworkHediff(true);
            }
        }

        //Check relatifs aux things virusés
        public void checkVirusedThings()
        {
            int GT = Find.TickManager.TicksGame;
        
            foreach (var t in listerVirusedThings.ToList())
            {
                CompSkyMind csm = t.TryGetComp<CompSkyMind>();

                if (csm == null)
                    continue;

                if (csm.hacked == 3 && GT >= csm.hackEndGT)
                {
                    csm.tempHackingEnding();

                    //reconnectDirectExternalController = true;
                }

                //Explosion androide infecté par un virus explosif
                if (csm.infectedExplodeGT != -1 && GT >= csm.infectedExplodeGT)
                {
                    csm.infectedExplodeGT = -1;
                    csm.Infected = -1;
                            

                    if (t is Pawn)
                    {
                        Pawn p = (Pawn)t;
                        Utils.makeAndroidBatteryOverload(p);
                    }
                    else
                    {
                        GenExplosion.DoExplosion(t.Position, t.Map, 3, DamageDefOf.Flame, null, -1, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
                        
                        Building build = (Building)t;
                        build.HitPoints =(int)(build.def.BaseMaxHitPoints * Rand.Range(0.05f,0.5f));
                    }
                }

                //Fin de la contamination vitale du dispositif
                if(csm.infectedEndGT != -1 && csm.infectedEndGT <= GT )
                {
                    csm.infectedEndGT = -1;
                    csm.Infected = -1;
                }
            }
        }

        private bool isThereSkyMindAntennaOrRelayInMap(Map map)
        {
            bool ok = false;
            if (map == null)
                return false;

            //Si ni relay no antennes sur la map alors androids sur cette derniere sont impactés
            if (listerSkyMindServers.ContainsKey(map) && listerSkyMindServers[map].Count > 0)
                ok = true;
            else if (listerSkyMindWANServers.ContainsKey(map) && listerSkyMindWANServers[map].Count > 0)
                ok = true;
            else if (listerSkyMindRelays.ContainsKey(map) && listerSkyMindRelays[map].Count > 0)
                ok = true;

            return ok;
        }


        public void checkSkyMindSignalPerformance()
        {
            //Maps
            foreach(var entry in listerSurrogateAndroids)
            {
                checkSkyMindSignalPerformanceEntry(entry);
            }
        }

        private void checkSkyMindSignalPerformanceEntry(KeyValuePair<string, List<Pawn>> entry)
        {
            bool forceRemoveHediff = false;
            string MUID = entry.Key;

            if (MUID == "caravan" && Settings.disableLowNetworkMalusInCaravans)
                forceRemoveHediff = true;

            Map map = entry.Key.getMapFromString();

            bool ok = isThereSkyMindAntennaOrRelayInMap(map);

            //Il y a une antenne permettant de relayer le skinMind sur la map en cours
            if (forceRemoveHediff || (ok && listerSurrogateAndroids.ContainsKey(MUID) && listerSurrogateAndroids[MUID].Count > 0))
            {
                foreach (var s in listerSurrogateAndroids[MUID])
                {
                    if (s.Dead)
                        continue;

                    Hediff he = s.health.hediffSet.GetFirstHediffOfDef(Utils.hediffLowNetworkSignal);
                    if (he != null)
                        s.health.RemoveHediff(he);
                }
            }
            else
            {
                //Pas d'antenne permetant de relayer le signal on va impacter els surrogates
                foreach (var s in listerSurrogateAndroids[MUID])
                {
                    //Les porteur de RX en sont exempté
                    if (s.Dead || s.health.hediffSet.GetFirstHediffOfDef(Utils.hediffHaveRXChip) != null)
                        continue;

                    Hediff he = s.health.hediffSet.GetFirstHediffOfDef(Utils.hediffLowNetworkSignal);
                    if (he == null)
                    {
                        //Log.Message("HEREEEEE");
                        s.health.AddHediff(Utils.hediffLowNetworkSignal);
                    }
                }
            }
        }

        public void checkSkyMindAutoReconnect()
        {
            foreach(var el in listerSkyMindable)
            {
                CompSkyMind csm = el.TryGetComp<CompSkyMind>();

                if (csm == null)
                    continue;

                if (csm.autoconn &&  !csm.connected)
                {
                    if (csm.canBeConnectedToSkyMind())
                        Utils.GCATPP.connectUser(el);
                }
            }
        }


        /*
         * SUite a decrementation nb serveurs check si on doit deconnecté des users aleatoirement
         */
        public void checkNeedRandomlyDisconnectUsers()
        {
            if(nbSlot < connectedThing.Count())
            {
                while(nbSlot < connectedThing.Count())
                {
                    Thing c = connectedThing.RandomElement();
                    disconnectUser(c);
                }
            }
        }

        public void reProcessNbSlot()
        {
            List<Building> toDel = null;
            int prevNbSlot = nbSlot;
            nbSlot = 0;
            foreach(var el in listerSkyMindServers)
            {
                foreach (var el2 in el.Value)
                {
                    if (el2 != null && el2.TryGetComp<CompPowerTrader>().PowerOn && !el2.IsBrokenDown())
                        nbSlot += 3;
                    else
                    {
                        if (toDel == null)
                            toDel = new List<Building>();

                        toDel.Add(el2);
                    }
                }

                if (toDel != null)
                {
                    foreach (var t in toDel)
                    {
                        el.Value.Remove(t);
                    }
                    toDel.Clear();
                }
            }


            foreach (var el in listerSkyMindWANServers)
            {
                foreach (var el2 in el.Value)
                {
                    if (el2 != null && el2.TryGetComp<CompPowerTrader>().PowerOn && !el2.IsBrokenDown())
                        nbSlot += 15;
                    else
                    {
                        if (toDel == null)
                            toDel = new List<Building>();

                        toDel.Add(el2);
                    }
                }

                if (toDel != null)
                {
                    foreach (var t in toDel)
                    {
                        el.Value.Remove(t);
                    }
                    toDel.Clear();
                }
            }

            //Si plus de support reseau skymind et core alors play alerte vocale
            if (prevNbSlot != 0 && nbSlot == 0 )
                Utils.playVocal("soundDefSkyCloudSkyMindNetworkOffline");
        }


        public void reProcessNbSecuritySlot()
        {
            List<Building> toDel = null;
            nbSecuritySlot = 0;
            foreach (var el in listerSecurityServers)
            {
                if (el != null && el.TryGetComp<CompPowerTrader>().PowerOn && !el.IsBrokenDown())
                {
                    nbSecuritySlot += Utils.nbSecuritySlotsGeneratedBy(el);
                }
                else
                {
                    if (toDel == null)
                        toDel = new List<Building>();

                    toDel.Add(el);
                }
            }

            if (toDel != null)
            {
                foreach (var el in toDel)
                {
                    listerSecurityServers.Remove(el);
                }

                toDel.Clear();
            }
        }

        public void reProcessNbHackingSlot()
        {
            List<Building> toDel = null;
            nbHackingSlot = 0;
            foreach (var el in listerHackingServers)
            {
                if (el != null && el.TryGetComp<CompPowerTrader>().PowerOn && !el.IsBrokenDown())
                {
                    nbHackingSlot += Utils.nbHackingSlotsGeneratedBy(el);
                }
                else
                {
                    if (toDel == null)
                        toDel = new List<Building>();

                    toDel.Add(el);
                }
            }

            if (toDel != null)
            {
                foreach (var el in toDel)
                {
                    listerHackingServers.Remove(el);
                }

                toDel.Clear();
            }
        }

        public void reProcessNbSkillSlot()
        {
            List<Building> toDel = null;
            nbSkillSlot = 0;
            foreach (var el in listerSkillServers)
            {
                if (el != null && el.TryGetComp<CompPowerTrader>().PowerOn && !el.IsBrokenDown())
                {
                    nbSkillSlot += Utils.nbSkillSlotsGeneratedBy(el);
                }
                else
                {
                    if (toDel == null)
                        toDel = new List<Building>();

                    toDel.Add(el);
                }
            }

            if (toDel != null)
            {
                foreach (var el in toDel)
                {
                    listerSkillServers.Remove(el);
                }

                toDel.Clear();
            }
        }

        public int getNbSlotAvailable()
        {
            return nbSlot;
        }

        public int getNbThingsConnected()
        {
            return connectedThing.Count();
        }

        public int getNbSkyMindUsers()
        {
            return listerSkyMindUsers.Count();
        }

        public int getNbSurrogateAndroids()
        {
            int ret = 0;
            foreach(var el in listerSurrogateAndroids)
            {
                ret+= el.Value.Count();
            }

            return ret;
        }

        public int getNbSlotSecurisedAvailable()
        {
            return nbSecuritySlot;
        }
        public int getNbHackingSlotAvailable()
        {
            return nbHackingSlot;
        }
        public int getNbSkillSlotAvailable()
        {
            return nbSkillSlot;
        }


        public int getNbSkillPoints()
        {
            return nbSkillPoints;
        }

        public int getNbHackingPoints()
        {
            return nbHackingPoints;
        }

        private void checkHeldThingsPawnInSkyMind(Pawn cpawn)
        {
            ThingOwner th = cpawn.carryTracker.GetDirectlyHeldThings();
            if (th != null)
            {
                foreach (Thing t in th)
                {
                    //Log.Message("2) " + t.LabelShortCap);
                    Pawn cp=null;
                    if(t is Pawn)
                        cp = (Pawn)t;
                    if (cp != null && !cp.Dead && (cp.def.defName.ContainsAny(Utils.ExceptionAndroidList) || cp.VXChipPresent()))
                    {
                        connectedThing.Add(cp);
                    }
                }
            }
        }

        public bool isConnectedToSkyMind(Thing colonist, bool tryAutoConnect=false)
        {
            if (connectedThing.Contains(colonist))
                return true;
            else
            {
                //Si un mind EST le skyCLoud hote est allumé ET booté alors oui considéré comme connected
                if(colonist is Pawn)
                {
                    CompSurrogateOwner cso = colonist.TryGetComp<CompSurrogateOwner>();
                    if(cso != null && cso.skyCloudHost != null)
                    {
                        CompSkyCloudCore csc = cso.skyCloudHost.TryGetComp<CompSkyCloudCore>();
                        if (csc != null && csc.Booted())
                            return true;
                    }
                    
                }
                if (tryAutoConnect)
                {
                    connectUser(colonist);
                    return isConnectedToSkyMind(colonist, false);
                }
                else
                    return false;
            }
        }

        public bool connectUser(Thing thing)
        {
            //Si déjà connecté return TRUE
            if (connectedThing.Contains(thing))
            {
                //Si surrogate on va en plus declencher un changement de Map
                if (thing is Pawn)
                {
                    Pawn pawn = (Pawn)thing;
                    if (pawn.IsSurrogateAndroid())
                    {
                        foreach (var entry in listerSurrogateAndroids.ToList())
                        {
                            string MUID = entry.Key;

                            if (entry.Value.Contains(pawn))
                            {
                                pushSurrogateAndroidNotifyMapChanged(pawn, MUID);
                            }
                        }
                    }
                }
                return true;
            }

            if(connectedThing.Count() >= nbSlot)
            {
                return false;
            }

            if (!connectedThing.Contains(thing))
            {
                connectedThing.Add(thing);
                if (thing is Pawn)
                {
                    Pawn pawn = (Pawn)thing;
                    CompAndroidState cas = pawn.TryGetComp<CompAndroidState>();
                    //Si surrogate ajout a la liste des surrogates (ET connecté au skyMind)
                    if (cas != null && cas.isSurrogate)
                    {
                        pushSurrogateAndroid(pawn);
                    }
                    else
                    {
                        pushSkyMindUser(pawn);
                    }

                    pawn.BroadcastCompSignal("SkyMindNetworkUserConnected");
                }
                else if(thing is Building)
                {
                    Building build = (Building)thing;
                    if (!listerConnectedDevices.Contains(build))
                        listerConnectedDevices.Add(thing);
                    build.BroadcastCompSignal("SkyMindNetworkUserConnected");
                }
            }

            //Log.Message("ICI BROADCAST => "+pawn.def.defName);

            return true;
        }

        public void disconnectUser(Thing thing)
        {
            if (connectedThing.Contains(thing))
            {
                connectedThing.Remove(thing);

                if (thing is Pawn)
                {
                    Pawn pawn = (Pawn)thing;
                    CompAndroidState cas = pawn.TryGetComp<CompAndroidState>();
                    //Si surrogate retrait de la liste des surrogates (ET connecté au skyMind)
                    if (cas != null && cas.isSurrogate)
                    {
                        string MUID = "caravan";
                        if (pawn.Map != null)
                            MUID = pawn.Map.GetUniqueLoadID();
                        popSurrogateAndroid(pawn, MUID);
                    }
                    else
                    {
                        popSkyMindUser(pawn);
                    }
                    pawn.BroadcastCompSignal("SkyMindNetworkUserDisconnected");
                }
                else if (thing is Building)
                {
                    Building build = (Building)thing;
                    if(listerConnectedDevices.Contains(build))
                        listerConnectedDevices.Remove(thing);
                    build.BroadcastCompSignal("SkyMindNetworkUserDisconnected");
                }
            }
        }


        public void pushSurrogateAndroid(Pawn sx)
        {
            string MUID = "caravan";
            if (sx.Map != null)
                MUID = sx.Map.GetUniqueLoadID();

            if (!listerSurrogateAndroids.ContainsKey(MUID))
                listerSurrogateAndroids[MUID] = new List<Pawn>();

            if (!listerSurrogateAndroids[MUID].Contains(sx))
                listerSurrogateAndroids[MUID].Add(sx);

            //Check rapide si hediff LowSignalSkyMin dpresent et doit etre enlevé pour eviter d'attendre la mise a jour prochaine toutes les X secs
            /*if (!isThereSkyMindAntennaOrRelayInMap(sx.Map))
            {
                //Pas de Connection ajout hediff lowSignalSkyMind
                Hediff he = sx.health.hediffSet.GetFirstHediffOfDef(Utils.hediffLowNetworkSignal);
                if (he == null)
                    sx.health.AddHediff(Utils.hediffLowNetworkSignal);
            }*/
        }

        public void pushSurrogateAndroidNotifyMapChanged(Pawn sx, string prevMUID)
        {
            string currMUID = "caravan";
            if (sx.Map != null)
                currMUID = sx.Map.GetUniqueLoadID();

            if(listerSurrogateAndroids.ContainsKey(prevMUID) && prevMUID != currMUID)
            {
                listerSurrogateAndroids[prevMUID].Remove(sx);
                if (!listerSurrogateAndroids.ContainsKey(currMUID))
                    listerSurrogateAndroids[currMUID] = new List<Pawn>();

                listerSurrogateAndroids[currMUID].Add( sx);
            }

            checkSkyMindSignalPerformance();
        }

        public void popSurrogateAndroid(Pawn sx, string MUID)
        {
            if (listerSurrogateAndroids.ContainsKey(MUID) && listerSurrogateAndroids[MUID].Contains(sx))
                listerSurrogateAndroids[MUID].Remove(sx);
        }

        public void pushSkyMindUser(Pawn sx)
        {
            if (!listerSkyMindUsers.Contains(sx))
                listerSkyMindUsers.Add(sx);
        }

        public void popSkyMindUser(Pawn sx)
        {
            if (listerSkyMindUsers.Contains(sx))
                listerSkyMindUsers.Remove(sx);
        }


        public List<Pawn> getRandomSurrogateAndroids(int nb, bool withoutVirus=true)
        {
            List<Pawn> ret = new List<Pawn>();
            List<Pawn> tmp = ret.ToList();

            //Linearisation des surrogates a travers les maps
            foreach(var e in listerSurrogateAndroids)
            {
                ret = ret.Concat(e.Value).ToList();
            }

            if (withoutVirus)
            {
                foreach(var e in tmp)
                {
                    CompSkyMind csm = e.TryGetComp<CompSkyMind>();
                    if(csm != null)
                    {
                        if (csm.Infected != -1)
                            ret.Remove(e);
                    }
                }
            }

            while(ret.Count > nb)
            {
                ret.Remove(ret.RandomElement());
            }

            return ret;
        }

        public List<Thing> getRandomDevices(int nb, bool withoutVirus = true)
        {
            List<Thing> ret = listerConnectedDevices.ToList();
            List<Thing> tmp = ret.ToList();

            if (withoutVirus)
            {
                foreach (var e in tmp)
                {
                    CompSkyMind csm = e.TryGetComp<CompSkyMind>();
                    if (csm != null)
                    {
                        if (csm.Infected != -1)
                            ret.Remove(e);
                    }
                }
            }

            while (ret.Count > nb)
            {
                ret.Remove(ret.RandomElement());
            }

            return ret;
        }

        public List<Pawn> getRandomSkyMindUsers(int nb)
        {
            List<Pawn> ret = listerSkyMindUsers.ToList();

            while (ret.Count > nb)
            {
                ret.Remove(ret.RandomElement());
            }

            return ret;
        }

        public Pawn getRandomSkyMindUser()
        {
            if (listerSkyMindUsers.Count == 0)
                return null;
            else
                return listerSkyMindUsers.RandomElement();
        }


        public void pushSkyMindServer(Building build)
        {
            if (!listerSkyMindServers.ContainsKey(build.Map))
                listerSkyMindServers[build.Map] = new List<Building>();

            if (build.TryGetComp<CompPowerTrader>().PowerOn)
            {
                listerSkyMindServers[build.Map].Add(build);
                reProcessNbSlot();
            }
        }

        public void popSkyMindServer(Building build, Map map)
        {
            if (listerSkyMindServers.ContainsKey(map) && listerSkyMindServers[map].Contains(build))
            {
                listerSkyMindServers[map].Remove(build);
                reProcessNbSlot();
            }
            checkNeedRandomlyDisconnectUsers();
        }

        public void pushSkyMindWANServer(Building build)
        {
            if (!listerSkyMindWANServers.ContainsKey(build.Map))
                listerSkyMindWANServers[build.Map] = new List<Building>();

            if (!listerSkyMindWANServers[build.Map].Contains(build) && build.TryGetComp<CompPowerTrader>().PowerOn)
            {
                listerSkyMindWANServers[build.Map].Add(build);
                reProcessNbSlot();
            }
        }

        public void popSkyMindWANServer(Building build, Map map)
        {
            if (listerSkyMindWANServers.ContainsKey(map) && listerSkyMindWANServers[map].Contains(build))
            {
                listerSkyMindWANServers[map].Remove(build);
                reProcessNbSlot();
            }
            checkNeedRandomlyDisconnectUsers();
        }

        public void pushReloadStation(Building build)
        {
            if (!listerReloadStation.ContainsKey(build.Map))
            {
                listerReloadStation[build.Map] = new List<Building>();
            }

            listerReloadStation[build.Map].Add( build);
        }

        public void popReloadStation(Building build, Map map)
        {
            if (listerReloadStation.ContainsKey(map) && listerReloadStation[map].Contains(build))
            {
                listerReloadStation[map].Remove(build);
            }
        }

        public Building getNearestReloadStation(Map map, Pawn android)
        {
            if (listerReloadStation.ContainsKey(map) || listerReloadStation[map].Count() == 0)
                return null;

            float dist = 0;
            Building ret = null;
            foreach(var el in listerReloadStation[map])
            {
                float curDist = android.Position.DistanceTo(el.Position);
                if (dist > curDist)
                {
                    dist = curDist;
                    ret = el;
                }
            }

            return ret;
        }

        public List<Building> getReloadStations(Map map)
        {
            if (listerReloadStation.ContainsKey(map))
                return listerReloadStation[map];
            else
                return null;
        }

        /*
         * Obtention station de rechargement ayant des slots libres
         */
        public Building getFreeReloadStation(Map map, Pawn android)
        {
            //Log.Message("Nb RS en stock " + listerReloadStation.Count);
            if (listerReloadStation.ContainsKey(map))
            {
                //Log.Message("ICI DISPONIBLE !!!!!");
                foreach(var el in listerReloadStation[map].OrderBy((Building b) => b.Position.DistanceTo(android.Position)))
                {
                    if (el == null || el.Destroyed || el.IsBrokenDown() || !el.TryGetComp<CompPowerTrader>().PowerOn || !el.Position.InAllowedArea(android))
                        continue;

                    CompReloadStation rs = el.TryGetComp<CompReloadStation>();
                    if (rs == null)
                        continue;
                    if (rs.getNbAndroidReloading(true) < 8)
                    {
                        IntVec3 freePlace = rs.getFreeReloadPlacePos(android);
                        //Check en plus si il y a une place de disponible du fait qu'une partie peut etre occulté par un mur etc...
                        if (freePlace != IntVec3.Invalid && android.CanReach(freePlace, PathEndMode.OnCell, Danger.Deadly))
                            return el;
                    }
                }
            }
            
            return null;
        }

        public int getNextSkyCloudID()
        {
            return SkyCloureCoreID;
        }

        public void incNextSkyCloudID()
        {
            SkyCloureCoreID++;
        }

        public int getNextSXID(int v)
        {
            if (v == 10)
                return S10NID;
            else if (v == 4)
                return S4NID;
            else if (v == 3)
                return S3NID;
            else if (v == 2)
                return S2NID;
            else if (v == 0)
                return S0NID;
            else if (v == 12)
                return SX2NID;
            else if (v == 120)
                return SX2KNID;
            else if (v == 13)
                return SX3NID;
            else if (v == 14)
                return SX4NID;
            else
                return S1NID;
        }

        public void incNextSXID(int v)
        {
            if (v == 10)
                S10NID++;
            else if (v == 4)
                S4NID++;
            else if (v == 3)
                S3NID++;
            else if (v == 2)
                S2NID++;
            else if (v == 0)
                S0NID++;
            else if (v == 12)
                SX2NID++;
            else if (v == 120)
                SX2KNID++;
            else if (v == 13)
                SX3NID++;
            else if (v == 14)
                SX4NID++;
            else
                S1NID++;
        }

        /*
         * Obtention des devices heat sensible ayant un hotLevel egal a celui spécifié
         */
        public List<Thing> getHeatSensitiveDevicesByHotLevel(Map map, int hotLevel)
        {
            List<Thing> ret = new List<Thing>();
            if (!listerHeatSensitiveDevices.ContainsKey(map))
                return null;
            foreach (var device in listerHeatSensitiveDevices[map])
            {
                if (device.TryGetComp<CompHeatSensitive>().hotLevel == hotLevel)
                    ret.Add(device);
            }
            return ret;
        }

        void checkRemoveAndroidFactions()
        {
            androidFactionCoalition = Find.FactionManager.FirstFactionOfDef(DefDatabase<FactionDef>.GetNamed("AndroidFriendliesAtlas"));
            if (androidFactionCoalition != null)
            {
                if (Settings.androidsAreRare)
                {
                    if (!androidFactionCoalition.defeated)
                    {
                        foreach (var el in Find.WorldObjects.SettlementBases.ToList())
                        {
                            if (el.Faction == androidFactionCoalition)
                            {
                                savedIASCoalition.Add(el);
                            }
                        }

                        if (savedIASCoalition.Count != 0)
                        {
                            foreach (var el in savedIASCoalition)
                            {
                                try
                                {
                                    Find.WorldObjects.SettlementBases.Remove(el);
                                }
                                catch (Exception)
                                {

                                }
                                try
                                {
                                    Find.WorldObjects.Remove(el);
                                }
                                catch(Exception)
                                {

                                }
                            }
                        }
                        androidFactionCoalition.defeated = true;
                    }
                    androidFactionCoalition.def.hidden = true;
                    androidFactionCoalition = null;
                }
                else
                {
                    androidFactionCoalition.def.hidden = false;
                    if (Utils.FACTIONDISCOVERY_LOADED)
                    {
                        try
                        {
                            MethodInfo method = Utils.factionDiscoveryAssembly.GetType("FactionDiscovery.MainUtilities").GetMethod("CreateBases", BindingFlags.Static | BindingFlags.NonPublic);

                            if (method != null)
                            {
                                androidFactionCoalition.def.hidden = false;
                                //Force faction discovery a recreer faction le cas echeant (faction detruite)
                                if (androidFactionCoalition.defeated)
                                {
                                    androidFactionCoalition.defeated = false;

                                    //Creation des bases par faction discovery
                                    method.Invoke(null, new object[] { androidFactionCoalition });
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Message("[ATPP] checkRemoveAndroidFactions.androidFactionCoalition " + e.Message + " " + e.StackTrace);
                        }
                    }
                    savedIASCoalition.Clear();
                }
            }

            androidFactionInsurrection = Find.FactionManager.FirstFactionOfDef(DefDatabase<FactionDef>.GetNamed("AndroidRebellionAtlas"));
            if (androidFactionInsurrection != null)
            {
                if (Settings.androidsAreRare)
                {
                    if (!androidFactionInsurrection.defeated)
                    {
                        foreach (var el in Find.WorldObjects.SettlementBases.ToList())
                        {
                            if (el.Faction == androidFactionInsurrection)
                            {
                                savedIASInsurrection.Add(el);
                            }
                        }

                        if (savedIASInsurrection.Count != 0)
                        {
                            foreach (var el in savedIASInsurrection)
                            {
                                try { 
                                    Find.WorldObjects.SettlementBases.Remove(el);
                                }
                                catch (Exception)
                                {

                                }

                                try { 
                                    Find.WorldObjects.Remove(el);
                                }
                                    catch (Exception)
                                {

                                }
                        }
                        }
                        androidFactionInsurrection.defeated = true;
                    }
                    androidFactionInsurrection.def.hidden = true;
                    androidFactionInsurrection = null;
                }
                else
                {
                    androidFactionInsurrection.def.hidden = false;
                    if (Utils.FACTIONDISCOVERY_LOADED) {
                        try
                        {
                            MethodInfo method = Utils.factionDiscoveryAssembly.GetType("FactionDiscovery.MainUtilities").GetMethod("CreateBases", BindingFlags.Static | BindingFlags.NonPublic);

                            if (method != null)
                            {
                                androidFactionInsurrection.def.hidden = false;
                                //Force faction discovery a recreer faction le cas echeant (faction detruite)
                                if (androidFactionInsurrection.defeated)
                                {
                                    androidFactionInsurrection.defeated = false;

                                    //Creation des bases par faction discovery
                                    method.Invoke(null, new object[] { androidFactionInsurrection });
                                }
                            }
                        }
                        catch(Exception e)
                        {
                            Log.Message("[ATPP] checkRemoveAndroidFactions. androidFactionInsurrection "+e.Message+" "+e.StackTrace);
                        }
                    }
                    savedIASInsurrection.Clear();
                }
            }

        }


        public int getNbDevices()
        {
            return listerConnectedDevices.Count();
        }


        public void pushHeatSensitiveDevices(Building build)
        {
            if (!listerHeatSensitiveDevices.ContainsKey(build.Map))
                listerHeatSensitiveDevices[build.Map] = new List<Building>();

            listerHeatSensitiveDevices[build.Map].Add(build);

        }

        public void popHeatSensitiveDevices(Building build, Map map)
        {
            if (!listerHeatSensitiveDevices.ContainsKey(map))
                return;

            listerHeatSensitiveDevices[map].Remove(build);
        }


         public void pushSecurityServer(Building build)
        {
            if (listerSecurityServers.Contains(build))
                return;

            listerSecurityServers.Add(build);
            reProcessNbSecuritySlot();
        }


        public void popSecurityServer(Building build)
        {
            if (!listerSecurityServers.Contains(build))
                return;

            listerSecurityServers.Remove(build);

            reProcessNbSecuritySlot();
        }

        public void pushSkillServer(Building build)
        {
            if (listerSkillServers.Contains(build))
                return;

            listerSkillServers.Add(build);
            reProcessNbSkillSlot();
        }

        public bool isThereSkillServers()
        {
            return listerSkillServers.Count() != 0;
        }


        public void popSkillServer(Building build)
        {
            if (!listerSkillServers.Contains(build))
                return;

            listerSkillServers.Remove(build);

            if (listerSkillServers.Count == 0 && nbSkillPoints != 0)
            {
                Find.LetterStack.ReceiveLetter("ATPP_LetterSkillPointsRemoved".Translate(), "ATPP_LetterSkillPointsRemovedDesc".Translate(nbSkillPoints), LetterDefOf.ThreatSmall);
                nbSkillPoints = 0;
            }

            reProcessNbSkillSlot();
        }

        public void pushHackingServer(Building build)
        {
            if (listerHackingServers.Contains(build))
                return;

            listerHackingServers.Add(build);
            reProcessNbHackingSlot();
        }


        public void popHackingServer(Building build)
        {
            if (!listerHackingServers.Contains(build))
                return;

            listerHackingServers.Remove(build);

            if (listerHackingServers.Count == 0 && nbHackingPoints != 0)
            {
                Find.LetterStack.ReceiveLetter("ATPP_LetterHackingPointsRemoved".Translate(), "ATPP_LetterHackingPointsRemovedDesc".Translate(nbHackingPoints), LetterDefOf.ThreatSmall);
                nbHackingPoints = 0;
            }

            reProcessNbHackingSlot();
        }


        public void pushSkyCloudCore(Building build)
        {
            if (listerSkyCloudCores.Contains(build))
                return;

            listerSkyCloudCores.Add(build);
        }


        public void popSkyCloudCoreAbs(Building build)
        {
            if (!listerSkyCloudCoresAbs.Contains(build))
                return;

            listerSkyCloudCoresAbs.Remove(build);
        }

        public void pushSkyCloudCoreAbs(Building build)
        {
            if (listerSkyCloudCoresAbs.Contains(build))
                return;

            listerSkyCloudCoresAbs.Add(build);
        }


        public void popSkyCloudCore(Building build)
        {
            if (!listerSkyCloudCores.Contains(build))
                return;

            listerSkyCloudCores.Remove(build);
        }

        public List<Building> getAvailableSkyCloudCores()
        {
            return listerSkyCloudCores;
        }

        public bool isThereSkyCloudCore()
        {
            return (listerSkyCloudCores.Count() > 0);
        }

        public bool isThereSkyCloudCoreAbs()
        {
            return (listerSkyCloudCoresAbs.Count() > 0);
        }

        public void pushRelayTower(Building build)
        {
            if (!listerSkyMindRelays.ContainsKey(build.Map))
                listerSkyMindRelays[build.Map] = new List<Building>();

            if (listerSkyMindRelays[build.Map].Contains(build))
                return;

            listerSkyMindRelays[build.Map].Add(build);
        }


        public void popRelayTower(Building build, Map map)
        {
            if (!listerSkyMindRelays.ContainsKey(map) || !listerSkyMindRelays[map].Contains(build))
                return;

            listerSkyMindRelays[map].Remove(build);
        }

        public void incHackingPoints(int nb)
        {
            if ((nbHackingPoints + nb) > nbHackingSlot)
            {
                if(!(nbHackingPoints > nbHackingSlot))
                    nbHackingPoints = nbHackingSlot;
            }
            else
                nbHackingPoints += nb;
        }

        public void decHackingPoints(int nb)
        {
            if (nbHackingPoints - nb < 0)
                nbHackingPoints = 0;
            else
                nbHackingPoints -= nb;
        }

        public void incSkillPoints(int nb)
        {
            if ((nbSkillPoints + nb) > nbSkillSlot)
            {
                if (!(nbSkillPoints > nbSkillSlot))
                    nbSkillPoints = nbSkillSlot;
            }
            else
                nbSkillPoints += nb;
        }

        public void decSkillPoints(int nb)
        {
            if (nbSkillPoints - nb < 0)
                nbSkillPoints = 0;
            else
                nbSkillPoints -= nb;
        }

        public void pushSkyMindable(Thing thing)
        {
            if (listerSkyMindable.Contains(thing))
                return;

            listerSkyMindable.Add(thing);
        }


        public void popSkyMindable(Thing thing)
        {
            if (!listerSkyMindable.Contains(thing))
                return;

            listerSkyMindable.Remove(thing);
        }

        public void pushVirusedThing(Thing thing)
        {
            if (listerVirusedThings.Contains(thing))
                return;

            listerVirusedThings.Add(thing);
        }


        public void popVirusedThing(Thing thing)
        {
            if (!listerVirusedThings.Contains(thing))
                return;

            listerVirusedThings.Remove(thing);
        }


        public bool pushLWPNAndroid(Building LWPN, Pawn android)
        {
            if (!listerLWPNAndroid.ContainsKey(LWPN))
                listerLWPNAndroid[LWPN] = new List<Pawn>();

            //Check si suffisament d'energie pour accueillir l'android
            int qtConsumed = Utils.getConsumedPowerByAndroid(android.def.defName);
            if (LWPN != null 
                && !LWPN.Destroyed 
                && ( LWPN.def.defName == "ARKPPP_LocalWirelessPowerEmitter" || ( LWPN.def.defName == "ARKPPP_LocalWirelessPortablePowerEmitter" && listerLWPNAndroid[LWPN].Count() < Settings.maxAndroidByPortableLWPN ))
                && LWPN.TryGetComp<CompPowerTrader>().PowerOn 
                && Utils.getCurrentAvailableEnergy(LWPN.PowerComp.PowerNet) - qtConsumed > 0)
            {
                listerLWPNAndroid[LWPN].Add(android);
                //incrémentation qt de courant consommé pat LWPN
                LWPN.TryGetComp<CompPowerTrader>().PowerOutput -= qtConsumed;
                return true;
            }
            else
                return false;
        }


        public void popLWPNAndroid(Building LWPN, Pawn android)
        {
            if (!listerLWPNAndroid.ContainsKey(LWPN))
                return;

            //incrémentation qt de courant consommé pat LWPN
            int qtConsumed = Utils.getConsumedPowerByAndroid(android.def.defName);
            LWPN.TryGetComp<CompPowerTrader>().PowerOutput += qtConsumed;

            listerLWPNAndroid[LWPN].Remove(android);
        }

        public int getNbAssistingMinds()
        {
            int nb = 0;
            foreach(var c in listerSkyCloudCores)
            {
                CompSkyCloudCore csc = c.TryGetComp<CompSkyCloudCore>();
                //Comptabilisation que si le systeme à bouté
                if(csc != null && csc.Booted())
                    nb +=csc.assistingMinds.Count();
            }

            return nb;
        }

        private void reset()
        {
            appliedSettingsOnReload = false;
            listerReloadStation.Clear();
            listerSkyMindServers.Clear();
            cacheATN.Clear();
            connectedThing.Clear();
            listerSkyMindWANServers.Clear();
            listerHeatSensitiveDevices.Clear();
            listerSecurityServers.Clear();
            listerHackingServers.Clear();
            listerSurrogateAndroids.Clear();
            listerSkyMindUsers.Clear();
            listerSkyMindRelays.Clear();
            listerSkyCloudCores.Clear();
            listerSkyMindable.Clear();
            listerConnectedDevices.Clear();
            listerVirusedThings.Clear();
            listerSkyCloudCoresAbs.Clear();
            listerSkillServers.Clear();
            savedIASInsurrection.Clear();
            savedIASCoalition.Clear();
            listerLWPNAndroid.Clear();
            QEESkinColor.Clear();
            QEEAndroidHairColor.Clear();
            QEEAndroidHair.Clear();
            VatGrowerLastPawnInProgress.Clear();
            VatGrowerLastPawnIsTX.Clear();
        }

        private void initNull()
        {
            appliedSettingsOnReload = false;

            if (listerReloadStation == null)
                listerReloadStation = new Dictionary<Map, List<Building>>();
            if (listerSkyMindServers == null)
                listerSkyMindServers =new Dictionary<Map, List<Building>>();
            if (listerSkyMindWANServers == null)
                listerSkyMindWANServers = new Dictionary<Map, List<Building>>();
            if (cacheATN == null)
                cacheATN = new Dictionary<Building, IEnumerable<IntVec3>>();
            if (connectedThing == null)
                connectedThing = new List<Thing>();
            if (listerHeatSensitiveDevices == null)
                listerHeatSensitiveDevices = new Dictionary<Map, List<Building>>();
            if (listerSecurityServers == null)
                listerSecurityServers = new List<Building>();
            if (listerHackingServers == null)
                listerHackingServers = new List<Building>();
            if (listerSurrogateAndroids == null)
                listerSurrogateAndroids = new Dictionary<string, List<Pawn>>();
            if (listerSkyMindUsers == null)
                listerSkyMindUsers = new List<Pawn>();
            if (listerSkyMindRelays == null)
                listerSkyMindRelays = new Dictionary<Map, List<Building>>();
            if (listerSkyCloudCores == null)
                listerSkyCloudCores = new List<Building>();
            if (listerSkyMindable == null)
                listerSkyMindable = new List<Thing>();
            if (listerConnectedDevices == null)
                listerConnectedDevices = new List<Thing>();
            if (listerVirusedThings == null)
                listerVirusedThings = new List<Thing>();
            if (listerSkyCloudCoresAbs == null)
                listerSkyCloudCoresAbs = new List<Building>();
            if (listerSkillServers == null)
                listerSkillServers = new List<Building>();
            if (savedIASCoalition == null)
                savedIASCoalition = new List<Settlement>();
            if (savedIASInsurrection == null)
                savedIASInsurrection = new List<Settlement>();
            if (listerLWPNAndroid == null)
                listerLWPNAndroid = new Dictionary<Building, List<Pawn>>();
            if (QEEAndroidHair == null)
                QEEAndroidHair = new Dictionary<string, string>();
            if (QEEAndroidHairColor == null)
                QEEAndroidHairColor = new Dictionary<string, string>();
            if (QEESkinColor == null)
                QEESkinColor = new Dictionary<string, string>();
            if (VatGrowerLastPawnInProgress == null)
                VatGrowerLastPawnInProgress = new Dictionary<string, Pawn>();
            if (VatGrowerLastPawnIsTX == null)
                VatGrowerLastPawnIsTX = new Dictionary<string, bool>();
        }

        public Faction androidFactionCoalition;
        public Faction androidFactionInsurrection;

        private List<Settlement> savedIASCoalition = new List<Settlement>();
        private List<Settlement> savedIASInsurrection = new List<Settlement>();

        private int S1NID = 1;
        private int S2NID = 1;
        private int S3NID = 1;
        private int S4NID = 1;
        private int S10NID = 1;
        private int S0NID = 1;
        private int SX2NID = 1;
        private int SX2KNID = 1;
        private int SX3NID = 1;
        private int SX4NID = 1;


        private int SkyCloureCoreID = 1;
        private bool appliedSettingsOnReload = false;

        public Dictionary<Map, List<Building>> listerHeatSensitiveDevices;

        private int nbHackingSlot = 0;
        private int nbSecuritySlot = 0;
        private int nbSkillSlot = 0;
        private int nbSlot = 0;
        private int nbHackingPoints = 0;
        private int nbSkillPoints = 0;

        private List<Thing> connectedThing = new List<Thing>();
        private Dictionary<Building, IEnumerable<IntVec3>> cacheATN;
        private List<Building> listerSkillServers = new List<Building>();
        private List<Building> listerSecurityServers = new List<Building>();
        private List<Building> listerHackingServers = new List<Building>();
        private List<Building> listerSkyCloudCores = new List<Building>();
        private List<Building> listerSkyCloudCoresAbs = new List<Building>();
        private List<Thing> listerSkyMindable = new List<Thing>();
        private List<Thing> listerConnectedDevices = new List<Thing>();
        private List<Thing> listerVirusedThings = new List<Thing>();


        public Dictionary<string, string> QEEAndroidHair = new Dictionary<string, string>();
        public Dictionary<string, string> QEEAndroidHairColor = new Dictionary<string, string>();
        public Dictionary<string, string> QEESkinColor = new Dictionary<string, string>();
        public Dictionary<string, Pawn> VatGrowerLastPawnInProgress = new Dictionary<string, Pawn>();
        public Dictionary<string, bool> VatGrowerLastPawnIsTX = new Dictionary<string, bool>();

        public Dictionary<Building, List<Pawn>> listerLWPNAndroid = new Dictionary<Building, List<Pawn>>();

        private Dictionary<Map,List<Building>> listerReloadStation = new Dictionary<Map,List<Building>>();
        private Dictionary<Map, List<Building>> listerSkyMindRelays = new Dictionary<Map, List<Building>>();
        private Dictionary<Map, List<Building>> listerSkyMindServers = new Dictionary<Map, List<Building>>();
        private Dictionary<Map, List<Building>> listerSkyMindWANServers = new Dictionary<Map, List<Building>>();
        private Dictionary<string, List<Pawn>> listerSurrogateAndroids = new Dictionary<string, List<Pawn>>();
        private List<Pawn> listerSkyMindUsers = new List<Pawn>();
        private Game game;

    }
}