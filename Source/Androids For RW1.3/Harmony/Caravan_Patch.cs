using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;
using RimWorld.Planet;

namespace MOARANDROIDS
{
    internal class Caravan_Patch

    {
        /*
         * PostFix évitant d'attribuer de need comfort et outdoor aux T1 et T2 et l'hygiene a l'ensemble des robots
         */
        [HarmonyPatch(typeof(Caravan), "get_NightResting")]
        public class NightResting_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ref bool __result, ref Caravan __instance)
            {
                if( !__instance.pawns.InnerListForReading.Any(p => !(Utils.ExceptionAndroidList.Contains(p.def.defName) || Utils.ExceptionAndroidAnimals.Contains(p.def.defName))))
                {
                    __result = false;
                }
            }
        }
    }
}