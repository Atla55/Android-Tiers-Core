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
    internal class ThoughtWorker_Precept_IdeoDiversity_Social_Patch
    {
        [HarmonyPatch(typeof(ThoughtWorker_Precept_IdeoDiversity_Social), "ShouldHaveThought")]
        public class TW_Precept_IdeoDiversity_Social_ShouldHaveThought
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn p, Pawn otherPawn, ref ThoughtState __result)
            {
                __result = p.Faction == otherPawn.Faction && !p.IsBasicAndroidTier() && p.Ideo != otherPawn.Ideo;
                return false;
            }
        }
    }
}