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
    internal class ThinkNode_ConditionalNeedPercentageAbove_Patch
    {
        /*
         * PostFix évitant d'attribuer de need comfort et outdoor aux T1 et T2 et l'hygiene a l'ensemble des robots
         */
        [HarmonyPatch(typeof(ThinkNode_ConditionalNeedPercentageAbove), "Satisfied")]
        public class Satisfied_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn pawn, ref bool __result, NeedDef  ___need)
            {
                if (pawn.needs.TryGetNeed(___need) == null)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }
    }
}