﻿using System;
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
                    Utils.CrafterDoctorJob = new HashSet<WorkGiverDef>();

                    Utils.thoughtDefVX0Puppet = DefDatabase<ThoughtDef>.GetNamed("ATPP_VX0PuppetThought");
                    Utils.dummyHeddif = DefDatabase<HediffDef>.GetNamed("ATPP_DummyHediff");
                    Utils.ResearchProjectSkyMindLAN = DefDatabase<ResearchProjectDef>.GetNamed("ATPP_ResearchSkyMindLAN");
                    Utils.ResearchProjectSkyMindWAN = DefDatabase<ResearchProjectDef>.GetNamed("ATPP_ResearchSkyMindWAN");
                    Utils.ResearchAndroidBatteryOverload = DefDatabase<ResearchProjectDef>.GetNamed("ATPP_ResearchBatteryOverload"); 
                    Utils.statDefAndroidTending = DefDatabase<StatDef>.GetNamed("ATPP_AndroidTendQuality");

                    Utils.statDefAndroidSurgerySuccessChance = DefDatabase<StatDef>.GetNamed("AndroidSurgerySuccessChance");


                    Utils.ATPP_MoteBIII = DefDatabase<ThingDef>.GetNamed("ATPP_MoteBIII", true);
                    Utils.ATPP_MoteBII = DefDatabase<ThingDef>.GetNamed("ATPP_MoteBII", true);
                    Utils.ATPP_MoteBI = DefDatabase<ThingDef>.GetNamed("ATPP_MoteBI", true);

                    //Prevent M7/M8 to generate error on item placements
                    StatDef csStatDef = DefDatabase<StatDef>.GetNamed("ConstructionSpeed",false);
                    if(csStatDef != null )
                        csStatDef.supressDisabledError = true;

                    //generating list of androids without skin
                    foreach (var el in Utils.ExceptionAndroidList)
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
                        Utils.ExceptionRepairableFrameworkHediff = new HashSet<HediffDef> { HediffDefOf.Scratch, HediffDefOf.Bite, HediffDefOf.Burn, HediffDefOf.Cut, HediffDefOf.Gunshot, HediffDefOf.Stab, HediffDefOf.SurgicalCut };

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

                    //Apply T1 tech research policy
                    try
                    {
                        Utils.applyT1TechResearch();
                    }
                    catch (Exception ex)
                    {
                        Log.Message("[ATPP] applyT1TechResearch " + ex.Message + " " + ex.StackTrace);
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
                                Log.Message("[ATPP] BlacklistingOtherAR  : " + el.defName);
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

                    Utils.ExceptionAndroidCanReloadWithPowerList.AddRange(Utils.ExceptionAndroidList);
                    Utils.ExceptionAndroidCanReloadWithPowerList.AddRange(Utils.ExceptionAndroidAnimalPowered);

                    Utils.ExceptionAndroidListAll.AddRange(Utils.ExceptionAndroidList);
                    Utils.ExceptionAndroidListAll.AddRange(Utils.ExceptionAndroidAnimals);

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
                                    CompProperties cp;
                                    //SkyMind
                                    if (td.defName != "M8Mech")
                                    {
                                        cp = new CompProperties();
                                        cp.compClass = typeof(CompSkyMind);
                                        td.comps.Add(cp);
                                    }

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

                    //Dynamic Psychology patching
                    if(Utils.PSYCHOLOGY_LOADED && Utils.psychologyAssembly != null)
                    {
                        try
                        {
                            var original = Utils.psychologyAssembly.GetType("Psychology.PsychologyBase").GetMethod("AnxietyEnabled", BindingFlags.Static | BindingFlags.Public);
                            var prefix = typeof(CPaths).GetMethod("Psychology_AnxietyEnabledPrefix", BindingFlags.Static | BindingFlags.Public);
                        }
                        catch(Exception e)
                        {
                            Log.Message("[ATPP] Psychology " + e.Message + " " + e.StackTrace);
                        }
                    }

                    //Dynamic MedicinePatch patching
                    /*if(Utils.MEDICINEPATCH_LOADED && Utils.medicinePatchAssembly != null) {
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
                    }*/

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

                    /*if (Utils.HOSPITALITY_LOADED)
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
                    }*/

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

                        //Generating listing of all Androids PKD
                        Utils.AndroidsAllPKD = Utils.AndroidsAllPKD.Concat(Utils.AndroidsPKDHostile)
                            .Concat(Utils.AndroidsPKDNeutral).Concat(Utils.AndroidsXISeriePKDHostile)
                            .Concat(Utils.AndroidsXISeriePKDNeutral).Concat(Utils.AndroidsXSeriePKDHostile).Concat(Utils.AndroidsXSeriePKDNeutral).ToList();

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

                    selTraits = new List<string> { "Brawler", "Ascetic", "Gourmand", "SlowLearner", "Undergrounder", "DislikesMen", "DislikesWomen" };

                    foreach (var st in selTraits)
                    {
                        t = DefDatabase<TraitDef>.GetNamed(st, false);
                        if (t != null)
                            Utils.RansomAddedBadTraitsAndroid.Add(t);
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



        private void removeBlacklistedAndroidsHediffs()
        {
            List<Hediff> toDel = new List<Hediff>();

            foreach(var map in Find.Maps)
            {
                foreach(var p in map.mapPawns.AllPawns)
                {
                    if ((p.RaceProps != null && p.RaceProps.FleshType == FleshTypeDefOfAT.AndroidTier) && p.health != null && p.health.hediffSet != null)
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
                        CompAndroidState cas = Utils.getCachedCAS(p);
                        if(cas != null && cas.surrogateController != null)
                        {
                            connectUser(p);
                        }
                    }
                }
            }
        }

        private void initCaravanAndWorldColonists()
        {
            foreach (var c in Find.World.worldObjects.Caravans)
            {
                foreach (var p in c.pawns)
                {
                    p.BroadcastCompSignal("AndroidTiers_CaravanInit");
                }
            }
            foreach (var p in Find.World.worldPawns.AllPawnsAlive)
            {
                if(p.Faction == Faction.OfPlayer)
                {
                    p.BroadcastCompSignal("AndroidTiers_CaravanInit");
                }
            }
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();

            initNull();
            checkRemoveAndroidFactions();
        }

        public override void LoadedGame()
        {
            base.LoadedGame();

            //Reconnection des surrogates en caravane
            reconnectSurrogatesInCaravans();
            initCaravanAndWorldColonists();
            removeBlacklistedAndroidsHediffs();
            checkRemoveAndroidFactions();
            Utils.resetCachedComps();
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
            Scribe_Collections.Look(ref this.externalSurrogateCJoiner, false, "ATPP_externalSurrogateCJoiner", LookMode.Deep);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                initNull();
            }
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            int CGT = Find.TickManager.TicksGame;

            //Each 10 seconds check network status
            if(CGT % 600 == 0)
            {
                checkVirusedThings();
                checkSkyMindAutoReconnect();

                if (Utils.POWERPP_LOADED)
                    checkDisconnectedFromLWPNAndroid();
            }

            if(CGT % 3600 == 0)
            {
                checkexternalSurrogateJoiner(CGT);
            }
        }

        /*
         * Check if there is pending external surrogate joiner (controller of surrogate captured and converted sucessfully)
         */
        public void checkexternalSurrogateJoiner(int GT)
        {
            for(int i= externalSurrogateCJoiner.Count-1; i >= 0; i--)
            {
                CompAndroidState cas = Utils.getCachedCAS(externalSurrogateCJoiner[i]);
                if(cas != null)
                {
                    if(GT >= cas.externalControllerConvertedJoinGT)
                    {
                        Map targetMap = Utils.getRichestMapOfPlayer();
                        IntVec3 intVec;
                        if( CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => targetMap.reachability.CanReachColony(c) && !c.Fogged(targetMap), targetMap, CellFinder.EdgeRoadChance_Neutral, out intVec))
                        {
                            if(externalSurrogateCJoiner[i].Faction != Faction.OfPlayer)
                            {
                                externalSurrogateCJoiner[i].SetFaction(Faction.OfPlayer);
                            }
                            HediffDef chipToAdd;
                            BodyPartRecord bpr = null;
                            bpr = externalSurrogateCJoiner[i].health.hediffSet.GetBrain();
                            //Add neural chip (VX1 or VX2)
                            if (Rand.Chance(0.95f))
                                chipToAdd = HediffDefOf.ATPP_HediffVX1Chip;
                            else
                                chipToAdd = HediffDefOf.ATPP_HediffVX2Chip;

                            Hediff he = externalSurrogateCJoiner[i].health.hediffSet.GetFirstHediffOfDef(chipToAdd);
                            if(he == null && bpr != null)
                            {
                                externalSurrogateCJoiner[i].health.AddHediff(chipToAdd, bpr);
                            }

                            GenSpawn.Spawn(externalSurrogateCJoiner[i], intVec, targetMap, WipeMode.Vanish);
                            Find.LetterStack.ReceiveLetter("ATPP_LetterExternalSurrogateControllerJoin".Translate(), "ATPP_LetterExternalSurrogateControllerJoinDesc".Translate(externalSurrogateCJoiner[i].LabelCap), LetterDefOf.PositiveEvent, externalSurrogateCJoiner[i]);
                        }
                        else
                        {
                            Find.LetterStack.ReceiveLetter("ATPP_LetterExternalSurrogateControllerCannotJoin".Translate(), "ATPP_LetterExternalSurrogateControllerCannotJoinDesc".Translate(externalSurrogateCJoiner[i].LabelCap), LetterDefOf.NegativeEvent, externalSurrogateCJoiner[i]);
                        }

                        externalSurrogateCJoiner.RemoveAt(i);
                    }
                }
            }
        }

        public void checkAssistingMindsBonus()
        {
            if(getNbAssistingMinds() >= 10)
            {
                foreach(var el in connectedThing)
                {
                    if(el is Pawn)
                    {
                        Pawn cp = (Pawn)el;
                        checkAssistingMindsBonusUnit(cp);
                    }
                }
            }
        }

        public void checkAssistingMindsBonusUnit(Pawn cp)
        {
            if (cp.health != null && cp.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_AssistingMinds) == null)
            {
                cp.health.AddHediff(HediffDefOf.ATPP_AssistingMinds);
            }
        }

        /*
         * Remove androids which cannot be powered and clean destroyed LWPN buildings
         */
        private List<Building> checkDisconnectedFromLWPNAndroidBuildingToDel = new List<Building>();
        private List<Pawn> checkDisconnectedFromLWPNAndroidToDel = new List<Pawn>();
        public void checkDisconnectedFromLWPNAndroid()
        {
            checkDisconnectedFromLWPNAndroidBuildingToDel.Clear();
            foreach (var el in listerLWPNAndroid)
            {
                if (el.Key.Destroyed)
                {
                    checkDisconnectedFromLWPNAndroidBuildingToDel.Add(el.Key);
                    continue;
                }

                float availablePower = Utils.getCurrentAvailableEnergy(el.Key.PowerComp.PowerNet);
                if (el.Value.Count == 0)
                    continue;

                int nbConn = 0;
                checkDisconnectedFromLWPNAndroidToDel.Clear();
                foreach (var android in el.Value.ToList())
                {
                    CompAndroidState cas = Utils.getCachedCAS(android);
                    if (cas != null && !cas.useBattery)
                    {
                        //el.Value.Remove(android);
                        checkDisconnectedFromLWPNAndroidToDel.Add(android);
                        continue;
                    }

                    CompPowerTrader cpt = Utils.getCachedCPT(el.Key);
                    bool nonFunctionalLWPN = el.Key.Destroyed || el.Key.IsBrokenDown() || !cpt.PowerOn;

                    //Déduction qt consommé par android
                    int qtConsummed = Utils.getConsumedPowerByAndroid(android.def.defName);
                    if (nonFunctionalLWPN || (availablePower - qtConsummed < 0) || (el.Key.def.defName != "ARKPPP_LocalWirelessPowerEmitter" && nbConn >= Settings.maxAndroidByPortableLWPN))
                    {
                        //el.Value.Remove(android);
                        checkDisconnectedFromLWPNAndroidToDel.Add(android);
                        cpt.PowerOutput += qtConsummed;
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
                //Effective removal
                foreach (var key in checkDisconnectedFromLWPNAndroidToDel)
                {
                    el.Value.Remove(key);
                }
            }
            //Effective removal of destroyed LWPN
            foreach (var key in checkDisconnectedFromLWPNAndroidBuildingToDel)
            {
                foreach(var el in listerLWPNAndroid[key])
                {
                    CompAndroidState cas = Utils.getCachedCAS(el);
                    if(cas != null)
                    {
                        cas.clearPPPState();
                    }
                }
                listerLWPNAndroid.Remove(key);
            }
        }

        //Check relatifs aux things virusés
        public void checkVirusedThings()
        {
            int GT = Find.TickManager.TicksGame;
            if(listerVirusedThings.Count == 0)
            {
                return;
            }
        
            foreach (var t in listerVirusedThings.ToList())
            {
                CompSkyMind csm = Utils.getCachedCSM(t);

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

        public bool isThereSkyMindAntennaOrRelayInMap(string MID)
        {
            bool ok = false;
            if (MID == null)
                return false;

            //Si ni relay no antennes sur la map alors androids sur cette derniere sont impactés
            if (listerSkyMindServers.TryGetValue(MID, out var value) && value.Count > 0)
                ok = true;
            else if (listerSkyMindWANServers.TryGetValue(MID, out value) && value.Count > 0)
                ok = true;
            else if (listerSkyMindRelays.TryGetValue(MID, out value) && value.Count > 0)
                ok = true;

            /*Log.Message("===== " + MID);
            foreach (var el in listerSkyMindServers)
            {
                foreach (var el2 in el.Value)
                {
                    Log.Message("=>" + el.Key + " : " + el2.LabelCap);
                }
            }*/

            return ok;
        }

        public void checkSkyMindAutoReconnect()
        {
            foreach(var el in listerSkyMindable)
            {
                Pawn cp=null;
                if (el is Pawn)
                    cp = (Pawn)el;
                CompSkyMind csm = Utils.getCachedCSM(cp);

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
            List<Thing> toDel = null;
            int prevNbSlot = nbSlot;
            nbSlot = 0;
            foreach(var el in listerSkyMindServers)
            {
                foreach (var el2 in el.Value)
                {
                    CompBuildingSkyMindLAN csml = Utils.getCachedCML(el2);
                    if (csml.isPowerOn())
                    {
                        nbSlot += 3;
                    }
                    else
                    {
                        if (toDel == null)
                            toDel = new List<Thing>();

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
                    CompPowerTrader cpt = Utils.getCachedCPT(el2);
                    if (el2 != null && cpt.PowerOn && !el2.IsBrokenDown())
                        nbSlot += 15;
                    else
                    {
                        if (toDel == null)
                            toDel = new List<Thing>();

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
                CompPowerTrader cpt = Utils.getCachedCPT(el);
                if (el != null && cpt.PowerOn && !el.IsBrokenDown())
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
                CompPowerTrader cpt = Utils.getCachedCPT(el);
                if (el != null && cpt.PowerOn && !el.IsBrokenDown())
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
                CompPowerTrader cpt = Utils.getCachedCPT(el);
                if (el != null && cpt.PowerOn && !el.IsBrokenDown())
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

        public bool isConnectedToSkyMind(Thing colonist, bool tryAutoConnect=false, bool broadcastEvent=true)
        {
            if (connectedThing.Contains(colonist))
                return true;
            else
            {
                //Si un mind ET le skyCLoud hote est allumé ET booté alors oui considéré comme connected
                if(colonist is Pawn)
                {
                    CompSurrogateOwner cso = Utils.getCachedCSO((Pawn)colonist);
                    if(cso != null && cso.skyCloudHost != null)
                    {
                        CompSkyCloudCore csc = Utils.getCachedCSC(cso.skyCloudHost);
                        if (csc != null && csc.Booted())
                            return true;
                    }
                }
                if (tryAutoConnect)
                {
                    connectUser(colonist);
                    return isConnectedToSkyMind(colonist, false, broadcastEvent);
                }
                else
                    return false;
            }
        }

        public bool connectUser(Thing thing, bool broadcastEvent=true)
        {
            bool containsThing = connectedThing.Contains(thing);
            //Si déjà connecté return TRUE
            if (containsThing)
            {
                return true;
            }

            //Nbslot available exceeded ? ==> no Skymind connection
            if(connectedThing.Count() >= nbSlot)
            {
                return false;
            }
            
            if (!containsThing)
            {
                connectedThing.Add(thing);
                if (thing is Pawn)
                {
                    Pawn pawn = (Pawn)thing;
                    CompAndroidState cas = Utils.getCachedCAS(pawn);
                    //Si surrogate ajout a la liste des surrogates (ET connecté au skyMind)
                    if (cas != null && cas.isSurrogate)
                    {
                        pushSurrogateAndroid(pawn);
                    }
                    else
                    {
                        pushSkyMindUser(pawn);
                    }
                    //Check if assisting minds bonus available
                    checkAssistingMindsBonusUnit(pawn);

                    if(broadcastEvent)
                        pawn.BroadcastCompSignal("SkyMindNetworkUserConnected");
                }
                else if(thing is Building)
                {
                    Building build = (Building)thing;
                    if (!listerConnectedDevices.Contains(build))
                        listerConnectedDevices.Add(thing);

                    if(broadcastEvent)
                        build.BroadcastCompSignal("SkyMindNetworkUserConnected");
                }
            }

            return true;
        }

        public void disconnectUser(Thing thing, bool manual=false, bool broadcastEvent=true)
        {
            if (connectedThing.Contains(thing))
            {
                string signal = "SkyMindNetworkUserDisconnected";
                if (manual)
                    signal = "SkyMindNetworkUserDisconnectedManually";

                connectedThing.Remove(thing);

                if (thing is Pawn)
                {
                    Pawn pawn = (Pawn)thing;
                    CompAndroidState cas = Utils.getCachedCAS(pawn);
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
                    if(broadcastEvent)
                        pawn.BroadcastCompSignal(signal);
                }
                else if (thing is Building)
                {
                    Building build = (Building)thing;
                    if(listerConnectedDevices.Contains(build))
                        listerConnectedDevices.Remove(thing);

                    if (broadcastEvent)
                        build.BroadcastCompSignal(signal);
                }
            }
        }


        public void pushSurrogateAndroid(Pawn sx)
        {
            string MUID = "caravan";
            if (sx.Map != null)
                MUID = sx.Map.GetUniqueLoadID();

            //Check presence of the surrogate in another map 
            foreach(var cmap in listerSurrogateAndroids)
            {
                Pawn s = null;
                cmap.Value.TryGetValue(sx, out s);
                if (sx != null)
                    cmap.Value.Remove(sx);
            }

            if (!listerSurrogateAndroids.ContainsKey(MUID))
                listerSurrogateAndroids[MUID] = new HashSet<Pawn>();

            if (!listerSurrogateAndroids[MUID].Contains(sx))
                listerSurrogateAndroids[MUID].Add(sx);
        }


        public void popSurrogateAndroid(Pawn sx, string MUID)
        {
            if (listerSurrogateAndroids.TryGetValue(MUID, out var value))
            {
                value.Remove(sx);
            }
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


        public HashSet<Pawn> getRandomSurrogateAndroids(int nb, bool withoutVirus = true)
        {
            HashSet<Pawn> ret = new HashSet<Pawn>();
            List<Pawn> tmp = ret.ToList();

            //Linearisation des surrogates a travers les maps
            ret = listerSurrogateAndroids.Values.SelectMany(x => x).ToHashSet();

            if (withoutVirus)
            {
                foreach (var e in tmp)
                {
                    CompSkyMind csm = Utils.getCachedCSM(e);
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

        public List<Thing> getRandomDevices(int nb, bool withoutVirus = true)
        {
            List<Thing> ret = listerConnectedDevices.ToList();
            List<Thing> tmp = ret.ToList();

            if (withoutVirus)
            {
                foreach (var e in tmp)
                {
                    CompSkyMind csm = Utils.getCachedCSM(e);
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


        public void pushSkyMindServer(Thing build, string MID)
        {
            //Log.Message("Push skymindServer " + MID);
            if (!listerSkyMindServers.ContainsKey(MID))
                listerSkyMindServers[MID] = new HashSet<Thing>();

            foreach(var el in listerSkyMindServers)
            {
                if (el.Value.Contains(build))
                {
                    el.Value.Remove(build);
                    break;
                }
            }

            listerSkyMindServers[MID].Add(build);
            reProcessNbSlot();
        }

        public void popSkyMindServer(Thing build)
        {
            //Log.Message("PopSkyMindServer");
            foreach (var el in listerSkyMindServers)
            {
                if (el.Value.Contains(build))
                {
                    el.Value.Remove(build);
                    reProcessNbSlot();
                    break;
                }
            }

            checkNeedRandomlyDisconnectUsers();
        }

        public void pushSkyMindWANServer(Thing build, string MID)
        {
            if (!listerSkyMindWANServers.ContainsKey(MID))
                listerSkyMindWANServers[MID] = new HashSet<Thing>();

            foreach (var el in listerSkyMindWANServers)
            {
                if (el.Value.Contains(build))
                {
                    el.Value.Remove(build);
                    break;
                }
            }

            listerSkyMindWANServers[MID].Add(build);
            reProcessNbSlot();
        }

        public void popSkyMindWANServer(Thing build)
        {
            foreach (var el in listerSkyMindWANServers)
            {
                if (el.Value.Contains(build))
                {
                    el.Value.Remove(build);
                    reProcessNbSlot();
                    break;
                }
            }

            checkNeedRandomlyDisconnectUsers();
        }

        public void pushReloadStation(Building build)
        {
            if (!listerReloadStation.ContainsKey(build.Map))
            {
                listerReloadStation[build.Map] = new HashSet<Building>();
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
            if (listerReloadStation.TryGetValue(map, out var value) || value.Count == 0)
                return null;

            float dist = 0;
            Building ret = null;
            foreach(var el in listerReloadStation[map])
            {
                float curDist = android.Position.DistanceToSquared(el.Position);
                if (dist > curDist)
                {
                    dist = curDist;
                    ret = el;
                }
            }

            return ret;
        }

        public HashSet<Building> getReloadStations(Map map)
        {
            if (listerReloadStation.TryGetValue(map, out var value))
                return value;
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
                foreach(var el in listerReloadStation[map].OrderBy((Building b) => b.Position.DistanceToSquared(android.Position)))
                {
                    CompPowerTrader cpt = Utils.getCachedCPT(el);
                    if (el == null || el.Destroyed || el.IsBrokenDown() || !cpt.PowerOn || !el.Position.InAllowedArea(android))
                        continue;

                    CompReloadStation rs = Utils.getCachedReloadStation(el);
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

        public void checkRemoveAndroidFactions()
        {
            if (!Settings.androidsAreRare)
                return;

            androidFactionCoalition = Find.FactionManager.FirstFactionOfDef(FactionDefOf.AndroidFriendliesAtlas);
            if (androidFactionCoalition != null)
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

            androidFactionInsurrection = Find.FactionManager.FirstFactionOfDef(FactionDefOf.AndroidRebellionAtlas);
            if (androidFactionInsurrection != null)
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


        public void pushSkyCloudCore(Thing build)
        {
            if (listerSkyCloudCores.Contains(build))
                return;

            listerSkyCloudCores.Add(build);
        }


        public void popSkyCloudCoreAbs(Thing build)
        {
            if (!listerSkyCloudCoresAbs.Contains(build))
                return;

            listerSkyCloudCoresAbs.Remove(build);
        }

        public void pushSkyCloudCoreAbs(Thing build)
        {
            if (listerSkyCloudCoresAbs.Contains(build))
                return;

            listerSkyCloudCoresAbs.Add(build);
        }


        public void popSkyCloudCore(Thing build)
        {
            if (!listerSkyCloudCores.Contains(build))
                return;

            listerSkyCloudCores.Remove(build);
        }

        public HashSet<Thing> getAvailableSkyCloudCores()
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

        public void pushRelayTower(Thing build, string MID)
        {
            if (!listerSkyMindRelays.ContainsKey(MID))
                listerSkyMindRelays[MID] = new HashSet<Thing>();

            if (listerSkyMindRelays[MID].Contains(build))
                return;

            listerSkyMindRelays[MID].Add(build);
        }


        public void popRelayTower(Thing build, string MID)
        {
            if (!listerSkyMindRelays.ContainsKey(MID) || !listerSkyMindRelays[MID].Contains(build))
                return;

            listerSkyMindRelays[MID].Remove(build);
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

            CompPowerTrader cpt = Utils.getCachedCPT(LWPN);

            //Check si suffisament d'energie pour accueillir l'android
            int qtConsumed = Utils.getConsumedPowerByAndroid(android.def.defName);
            if (LWPN != null 
                && !LWPN.Destroyed 
                && ( LWPN.def.defName == "ARKPPP_LocalWirelessPowerEmitter" || ( LWPN.def.defName == "ARKPPP_LocalWirelessPortablePowerEmitter" && listerLWPNAndroid[LWPN].Count() < Settings.maxAndroidByPortableLWPN ))
                && cpt.PowerOn 
                && Utils.getCurrentAvailableEnergy(LWPN.PowerComp.PowerNet) - qtConsumed > 0)
            {
                listerLWPNAndroid[LWPN].Add(android);
                //incrémentation qt de courant consommé pat LWPN
                cpt.PowerOutput -= qtConsumed;
                return true;
            }
            else
                return false;
        }


        public void popLWPNAndroid(Building LWPN, Pawn android)
        {
            if (!listerLWPNAndroid.ContainsKey(LWPN))
                return;

            CompPowerTrader cpt = Utils.getCachedCPT(LWPN);
            //incrémentation qt de courant consommé pat LWPN
            int qtConsumed = Utils.getConsumedPowerByAndroid(android.def.defName);
            cpt.PowerOutput += qtConsumed;

            listerLWPNAndroid[LWPN].Remove(android);
        }

        public int getNbAssistingMinds()
        {
            int nb = 0;
            foreach(var c in listerSkyCloudCores)
            {
                CompSkyCloudCore csc = Utils.getCachedCSC(c);
                //Comptabilisation que si le systeme à bouté
                if(csc != null && csc.Booted())
                    nb +=csc.assistingMinds.Count();
            }

            return nb;
        }

        public void reset()
        {
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
            Utils.listerDownedSurrogatesThing.Clear();
            Utils.listerDownedSurrogatesCAS.Clear();
        }

        private void initNull()
        { 
            if (listerReloadStation == null)
                listerReloadStation = new Dictionary<Map, HashSet<Building>>();
            if (listerSkyMindServers == null)
                listerSkyMindServers =new Dictionary<string, HashSet<Thing>>();
            if (listerSkyMindWANServers == null)
                listerSkyMindWANServers = new Dictionary<string, HashSet<Thing>>();
            if (cacheATN == null)
                cacheATN = new Dictionary<Building, IEnumerable<IntVec3>>();
            if (connectedThing == null)
                connectedThing = new HashSet<Thing>();
            if (listerHeatSensitiveDevices == null)
                listerHeatSensitiveDevices = new Dictionary<Map, List<Building>>();
            if (listerSecurityServers == null)
                listerSecurityServers = new HashSet<Building>();
            if (listerHackingServers == null)
                listerHackingServers = new HashSet<Building>();
            if (listerSurrogateAndroids == null)
                listerSurrogateAndroids = new Dictionary<string, HashSet<Pawn>>();
            if (listerSkyMindUsers == null)
                listerSkyMindUsers = new HashSet<Pawn>();
            if (listerSkyMindRelays == null)
                listerSkyMindRelays = new Dictionary<string, HashSet<Thing>>();
            if (listerSkyCloudCores == null)
                listerSkyCloudCores = new HashSet<Thing>();
            if (listerSkyMindable == null)
                listerSkyMindable = new HashSet<Thing>();
            if (listerConnectedDevices == null)
                listerConnectedDevices = new HashSet<Thing>();
            if (listerVirusedThings == null)
                listerVirusedThings = new HashSet<Thing>();
            if (listerSkyCloudCoresAbs == null)
                listerSkyCloudCoresAbs = new HashSet<Thing>();
            if (listerSkillServers == null)
                listerSkillServers = new HashSet<Building>();
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
            if (externalSurrogateCJoiner == null)
                externalSurrogateCJoiner = new List<Pawn>();
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

        public Dictionary<Map, List<Building>> listerHeatSensitiveDevices;

        private int nbHackingSlot = 0;
        private int nbSecuritySlot = 0;
        private int nbSkillSlot = 0;
        private int nbSlot = 0;
        private int nbHackingPoints = 0;
        private int nbSkillPoints = 0;


        public List<Pawn> externalSurrogateCJoiner = new List<Pawn>();
        public HashSet<Thing> connectedThing = new HashSet<Thing>();
        private Dictionary<Building, IEnumerable<IntVec3>> cacheATN;
        private HashSet<Building> listerSkillServers = new HashSet<Building>();
        private HashSet<Building> listerSecurityServers = new HashSet<Building>();
        private HashSet<Building> listerHackingServers = new HashSet<Building>();
        private HashSet<Thing> listerSkyCloudCores = new HashSet<Thing>();
        private HashSet<Thing> listerSkyCloudCoresAbs = new HashSet<Thing>();
        private HashSet<Thing> listerSkyMindable = new HashSet<Thing>();
        private HashSet<Thing> listerConnectedDevices = new HashSet<Thing>();
        private HashSet<Thing> listerVirusedThings = new HashSet<Thing>();


        public Dictionary<string, string> QEEAndroidHair = new Dictionary<string, string>();
        public Dictionary<string, string> QEEAndroidHairColor = new Dictionary<string, string>();
        public Dictionary<string, string> QEESkinColor = new Dictionary<string, string>();
        public Dictionary<string, Pawn> VatGrowerLastPawnInProgress = new Dictionary<string, Pawn>();
        public Dictionary<string, bool> VatGrowerLastPawnIsTX = new Dictionary<string, bool>();

        public Dictionary<Building, List<Pawn>> listerLWPNAndroid = new Dictionary<Building, List<Pawn>>();

        private Dictionary<Map, HashSet<Building>> listerReloadStation = new Dictionary<Map, HashSet<Building>>();
        private Dictionary<string, HashSet<Thing>> listerSkyMindRelays = new Dictionary<string, HashSet<Thing>>();
        private Dictionary<string, HashSet<Thing>> listerSkyMindServers = new Dictionary<string, HashSet<Thing>>();
        private Dictionary<string, HashSet<Thing>> listerSkyMindWANServers = new Dictionary<string, HashSet<Thing>>();
        private Dictionary<string, HashSet<Pawn>> listerSurrogateAndroids = new Dictionary<string, HashSet<Pawn>>();
        private HashSet<Pawn> listerSkyMindUsers = new HashSet<Pawn>();

        private Game game;

    }
}