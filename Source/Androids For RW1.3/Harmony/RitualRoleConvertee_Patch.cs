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
    internal class RitualRoleConvertee_Patch
    {
        [HarmonyPatch(typeof(RitualRoleConvertee), "AppliesToPawn")]
        public class PawnObserver_PossibleToObserve_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn p, out string reason, LordJob_Ritual ritual, RitualRoleAssignments assignments, Precept_Ritual precept, bool skipReason, ref bool __result)
            {
                reason = null;
                if (p == null || p.IsBasicAndroidTier() || p.IsSurrogateAndroid(false, true))
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
    }
}