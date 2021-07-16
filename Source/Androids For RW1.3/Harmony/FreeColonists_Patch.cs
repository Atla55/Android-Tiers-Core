using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace MOARANDROIDS
{
    internal class Thing_Patch

    {
        /*
         * PostFix évitant d'attribuer de need comfort et outdoor aux T1 et T2 et l'hygiene a l'ensemble des robots
         */
        [HarmonyPatch(typeof(Thing), "Ingested")]
        public class Ingested_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn ingester, float nutritionWanted, ref float __result)
            {
                try
                {
                    if ((Utils.ExceptionAndroidList.Contains(ingester.def.defName)))
                    {
                            if (Settings.androidNeedToEatMore)
                            __result *= 0.5f;
                    }
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] Thing.Ingested : " + e.Message + " - " + e.StackTrace);
                }
            }
        }

        /*
         * Pawn inside SkyCloud are not interpreted and suspended 
         */
        [HarmonyPatch(typeof(Thing), "get_Suspended")]
        public class Suspended_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Thing __instance, ref bool __result)
            {
                if (__instance is Pawn) {
                    CompSurrogateOwner cso = ((Pawn)__instance).TryGetComp<CompSurrogateOwner>();
                    if (cso != null && cso.skyCloudHost != null)
                        __result = true;
                }
            }
        }
    }
}