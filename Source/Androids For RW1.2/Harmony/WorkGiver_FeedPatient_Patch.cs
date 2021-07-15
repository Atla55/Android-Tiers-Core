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
    internal class WorkGiver_FeedPatient_Patch

    {
        /*
         * Allow crafters to do doctor jobs (for androids)
         */
        [HarmonyPatch(typeof(WorkGiver_FeedPatient), "HasJobOnThing")]
        public class HasJobOnThing_Patch
        {
            [HarmonyPostfix]
            public static void ListenerPostfix(Pawn pawn, Thing t, bool forced, ref bool __result, WorkGiver_FeedPatient __instance)
            {
                Utils.genericPostFixExtraCrafterDoctorJobs(pawn, t, forced, ref __result, __instance);
            }

            [HarmonyPrefix]
            public static bool ListenerPrefix(Pawn pawn, Thing t, bool forced, ref bool __result, WorkGiver_FeedPatient __instance)
            {
                //On va en plus checker si le eater est pas un android chargé sur un wireless powergrid pas de feed patients co 
                Pawn pawn2 = t as Pawn;
                if (pawn2 != null && pawn2.IsAndroidTier())
                {
                    CompAndroidState cas = pawn2.TryGetComp<CompAndroidState>();
                    if (cas != null)
                    {
                        if (cas.connectedLWPNActive && cas.connectedLWPN != null)
                        {
                            __result = false;
                            return false;
                        }
                    }
                }
                return true;
            }
        }
    }
}