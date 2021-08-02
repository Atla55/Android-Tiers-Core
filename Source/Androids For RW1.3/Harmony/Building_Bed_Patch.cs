using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MOARANDROIDS
{
    internal class Building_Bed_Patch

    {

        [HarmonyPatch(typeof(CompAssignableToPawn_Bed), "get_AssigningCandidates")]
        public class get_AssigningCandidates
        {
            private static void addInactiveSurrogates(ref List<Pawn> lst, Map map, bool M7, CompAssignableToPawn_Bed inst)
            {
                foreach(var cas in Utils.listerDownedSurrogatesCAS)
                {
                    Pawn cp =(Pawn)cas.parent;
                    if (cp != null)
                        Log.Message("=>" + cp.LabelCap);
                    if (cas != null && cp != null && inst.CanAssignTo(cp) && cas.isSurrogate && cas.surrogateController == null && !cas.isOrganic && (!M7 || (cp.def.defName == Utils.M7 || cp.def.defName == Utils.M8)))
                    {
                        lst.Add(cp);
                    }
                }
            }

            [HarmonyPostfix]
            public static void Listener(ref IEnumerable<Pawn> __result, CompAssignableToPawn_Bed __instance)
            {
                IEnumerable<Pawn> orig = __result;
                try
                {
                    Building_Bed bed = (Building_Bed)__instance.parent;
                    if (!bed.Spawned)
                        return;

                    if (bed.def.defName == "ATPP_AndroidPod")
                    {
                        List<Pawn> lst = new List<Pawn>();
                        foreach (var el in __result)
                        {
                            if (el.RaceProps.FleshType == FleshTypeDefOfAT.AndroidTier && (el.def != ThingDefOfAT.M7Mech && el.def != ThingDefOfAT.M8Mech))
                                lst.Add(el);
                        }

                        //Si option masquant les surrogates activé alors ajout de ces derniers à la fin
                        if (Settings.hideInactiveSurrogates)
                            addInactiveSurrogates(ref lst, bed.Map, false, __instance);

                        __result = lst;
                    }
                    else if (bed.def.defName == "ATPP_AndroidPodMech")
                    {
                        List<Pawn> lst = new List<Pawn>();
                        foreach (var el in __result)
                        {
                            if (el.def == ThingDefOfAT.M7Mech || el.def == ThingDefOfAT.M8Mech)
                                lst.Add(el);
                        }
                        //Si option masquant les surrogates activé alors ajout de ces derniers à la fin
                        if (Settings.hideInactiveSurrogates)
                            addInactiveSurrogates(ref lst, bed.Map, true, __instance);

                        __result = lst;
                    }
                    else
                    {
                        List<Pawn> lst = new List<Pawn>();
                        foreach (var el in __result)
                        {
                            if (el.def != ThingDefOfAT.M7Mech && el.def != ThingDefOfAT.M8Mech)
                                lst.Add(el);
                        }

                        //Si option masquant les surrogates activé alors ajout de ces derniers à la fin
                        if (Settings.hideInactiveSurrogates)
                            addInactiveSurrogates(ref lst, bed.Map, false, __instance);

                        __result = lst;
                    }
                }
                catch(Exception e)
                {
                    __result = orig;
                    Log.Message("[ATPP] Building_Bed get_AssigningCandidates" + e.Message + " " + e.StackTrace);
                }
            }
        }


        [HarmonyPatch(typeof(Building_Bed), "GetFloatMenuOptions")]
        public class GetFloatMenuOptions_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn myPawn, Building_Bed __instance, ref IEnumerable<FloatMenuOption>  __result)
            {
                if ((__instance.def.defName != "ATPP_AndroidPod" && __instance.def.defName != "ATPP_AndroidPodMech") || __instance.Medical || (myPawn.ownership != null && myPawn.ownership.OwnedBed != null && myPawn.ownership.OwnedBed != __instance))
                    return;

                if(__result == null)
                    __result = new List<FloatMenuOption>();

                FloatMenuOption failureReason = GetFailureReason(__instance, myPawn);
                if (failureReason != null)
                {
                    __result = __result.AddItem(failureReason);
                }
                else
                {

                    __result = __result.AddItem(new FloatMenuOption("ATPP_ForceReload".Translate(), delegate () {

                        //Affectation du pod a myPawn pour eviter le rehet du job
                        myPawn.ownership.ClaimBedIfNonMedical(__instance);

                        Job job = new Job(JobDefOfAT.ATPP_GoReloadBattery, new LocalTargetInfo(__instance));
                        myPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);

                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
            }

            private static FloatMenuOption GetFailureReason(Building_Bed bed, Pawn myPawn)
            {
                if (!myPawn.CanReach(bed, PathEndMode.InteractionCell, Danger.Some, false, false, TraverseMode.ByPawn))
                {
                    return new FloatMenuOption("CannotUseNoPath".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                }
                if (bed.Spawned && Find.World.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare))
                {
                    return new FloatMenuOption("CannotUseSolarFlare".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                }
                CompPowerTrader cpt = Utils.getCachedCPT(bed);
                if (!cpt.PowerOn)
                {
                    return new FloatMenuOption("CannotUseNoPower".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                }
                if (!(myPawn.RaceProps.FleshType == FleshTypeDefOfAT.AndroidTier ))
                {
                    return new FloatMenuOption("ATPP_CanOnlyBeUsedByAndroid".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                }

                CompAndroidState ca = Utils.getCachedCAS(myPawn);
                if (ca == null || !ca.UseBattery)
                    return new FloatMenuOption("ATPP_CannotUseBecauseNotInBatteryMode".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);

                return null;
            }
        }

    }
}