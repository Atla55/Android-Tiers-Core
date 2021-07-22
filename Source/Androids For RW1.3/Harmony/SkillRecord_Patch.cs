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
    /*
     * Prevent Androids from losing skills over time
     */
    internal class SkillRecord_Patch
    {
        [HarmonyPatch(typeof(SkillRecord), "Interval")]
        public class SkillRecord_Interval_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn ___pawn )
            {
                if (___pawn.IsAndroidTier())
                    return false;
                return true;
            }
        }

    }
}