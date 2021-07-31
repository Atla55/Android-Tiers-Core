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
        [HarmonyPatch(typeof(Caravan), "get_NightResting")]
        public class NightResting_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ref bool __result, ref Caravan __instance)
            {
                if (!__result)
                    return;

                if( !__instance.pawns.InnerListForReading.Any(p => !(p.RaceProps.FleshType == FleshTypeDefOfAT.AndroidTier)))
                {
                    __result = false;
                }
            }
        }
    }
}