/*using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MOARANDROIDS
{
    internal class PawnBioAndNameGenerator_Patch

    {
        [HarmonyPatch(typeof(PawnBioAndNameGenerator), "GiveShuffledBioTo")]
        public class CountAddedParts_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn pawn, FactionDef factionType, string requiredLastName, List<BackstoryCategoryFilter> backstoryCategories, bool forceNoBackstory)
            {
                var lst = "";
                foreach(var el in backstoryCategories)
                {
                    foreach(var el2 in el.categories)
                        lst += el2 + ",";
                }
                Log.Message("=> " + pawn.LabelCap + " " + pawn.kindDef.defName+" "+lst+" "+factionType.defName);
                return true;
            }
        }
    }
}*/