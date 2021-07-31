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
                    if ((ingester.IsAndroidTier()))
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
    }
}