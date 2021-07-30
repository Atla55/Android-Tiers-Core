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
    internal class WorkGiver_TendOther_Patch

    {
        /*
         * Allow crafters to do doctor jobs (for androids)
         */
        [HarmonyPatch(typeof(WorkGiver_TendOther), "HasJobOnThing")]
        public class HasJobOnThing_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn pawn, Thing t, bool forced, ref bool __result, WorkGiver_TendOther __instance)
            {
                bool ret = Utils.genericPostFixExtraCrafterDoctorJobs(pawn, t, forced, __instance);
                if (!ret)
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
    }
}