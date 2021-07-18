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
    internal class FactionGenerator_Patch

    {
        [HarmonyPatch(typeof(FactionGenerator), "GenerateFactionsIntoWorld")]
        public class Ingested_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Dictionary<FactionDef, int> factionCounts)
            {
                try
                {
                      Utils.GCATPP.checkRemoveAndroidFactions();
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] FactionGenerator.GenerateFactionsIntoWorld : " + e.Message + " - " + e.StackTrace);
                }
            }
        }
    }
}