﻿using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MOARANDROIDS
{
    internal class WorkGiver_DoBill_Patch

    {

        [HarmonyPatch(typeof(WorkGiver_DoBill), "AddEveryMedicineToRelevantThings")]
        public class AddEveryMedicineToRelevantThings_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn pawn, Thing billGiver, List<Thing> relevantThings, Predicate<Thing> baseValidator, Map map)
            {
                try
                {
                    if (billGiver is Pawn)
                    {
                        Pawn patient = (Pawn)billGiver;

                        //We withdraw medicine other than nanokits
                        if (patient.RaceProps.FleshType == FleshTypeDefOfAT.AndroidTier)
                        {
                            for (int i = relevantThings.Count - 1; i >= 0; i--)
                            {
                                if (!Utils.ExceptionNanoKits.Contains(relevantThings[i].def.defName))
                                    relevantThings.RemoveAt(i);
                            }
                        }
                        //We remove the nanokits
                        else
                        {
                            for (int i = relevantThings.Count - 1; i >= 0; i--)
                            {
                                if (Utils.ExceptionNanoKits.Contains(relevantThings[i].def.defName))
                                    relevantThings.RemoveAt(i);
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] WorkGiver_DoBill.AddEveryMedicineToRelevantThings " + e.Message + " " + e.StackTrace);
                }
            }
        }


        /*
         * Allow crafters to do doctor jobs (for androids)
         */
        [HarmonyPatch(typeof(WorkGiver_DoBill), "JobOnThing")]
        public class JobOnThing_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn pawn, Thing thing, bool forced, ref Job __result, WorkGiver_DoBill __instance)
            {
                try
                {
                    if (!Settings.androidsCanOnlyBeHealedByCrafter)
                    {
                        if (__instance.def.workType == Utils.WorkTypeDefSmithing && Utils.CrafterDoctorJob.Contains(__instance.def))
                            __result = null;

                        return;
                    }

                    Pawn cp = null;
                    if(thing is Pawn)
                        cp = (Pawn)thing;

                    //Normal doctor we go out if t is an android
                    if (__instance.def.workType == WorkTypeDefOf.Doctor)
                    {
                        if (cp != null && (cp.RaceProps.FleshType == FleshTypeDefOfAT.AndroidTier))
                            __result = null;
                    }
                    else
                    {
                        if (Utils.CrafterDoctorJob.Contains(__instance.def))
                        {
                            //Crafter we go out if patient not an android or a cyber animal
                            if (cp != null && (cp.RaceProps.FleshType == FleshTypeDefOfAT.AndroidTier))
                            {
                                CompSurrogateOwner cso = Utils.getCachedCSO(pawn);
                                

                                if (cso == null || !cso.repairAndroids)
                                    __result = null;
                            }
                            else
                                __result = null;
                        }
                    }
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] WorkGiver_DoBill.JobOnThing " + e.Message + " " + e.StackTrace);
                }
            }
        }
    }
}