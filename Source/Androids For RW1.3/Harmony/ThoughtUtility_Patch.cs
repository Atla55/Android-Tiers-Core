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
    internal class ThoughtUtility_Patch

    {
        [HarmonyPatch(typeof(ThoughtUtility), "GiveThoughtsForPawnExecuted")]
        public class GiveThoughtsForPawnExecuted_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn victim, PawnExecutionKind kind)
            {
                if (victim.IsBasicAndroidTier())
                    return false;
                else
                    return true;
            }
        }

        [HarmonyPatch(typeof(ThoughtUtility), "GiveThoughtsForPawnOrganHarvested")]
        public class GiveThoughtsForPawnOrganHarvested_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn victim)
            {
                if (victim.IsBasicAndroidTier())
                    return false;
                else
                    return true;
            }
        }


        [HarmonyPatch(typeof(ThoughtUtility), "CanGetThought")]
        public class CanGetThought_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn pawn, ThoughtDef def, ref bool __result)
            {
                if (pawn.IsBasicAndroidTier())
                {
                    if (def == ThoughtDefOf.DeadMansApparel || def == ThoughtDefOf.HumanLeatherApparelSad)
                        __result = false;
                    if (def == ThoughtDefOf.HumanLeatherApparelHappy)
                        __result = true;
                }
            }
        }

    }
}