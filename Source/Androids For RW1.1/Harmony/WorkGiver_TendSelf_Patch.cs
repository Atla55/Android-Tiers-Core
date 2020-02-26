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
    internal class WorkGiver_TendSelf_Patch

    {
        /*
         * Allow crafters to do doctor jobs (for androids)
         */
        [HarmonyPatch(typeof(WorkGiver_TendSelf), "HasJobOnThing")]
        public class HasJobOnThing_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn pawn, Thing t, bool forced, ref bool __result, WorkGiver_TendSelf __instance)
            {
                Utils.genericPostFixExtraCrafterDoctorJobs(pawn, t, forced, ref __result, __instance);
            }
        }
    }
}