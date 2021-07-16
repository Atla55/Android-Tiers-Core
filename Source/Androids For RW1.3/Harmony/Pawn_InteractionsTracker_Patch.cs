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
    internal class Pawn_InteractionsTracker_Patch

    {
        /*
         * PRIMARY remove androids to be have romance
         */
        [HarmonyPatch(typeof(Pawn_InteractionsTracker), "TryInteractWith")]
        public class TryInteractWith_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn recipient, InteractionDef intDef, Pawn ___pawn, ref bool __result)
            {
                if ((___pawn.IsBasicAndroidTier() || recipient.IsBasicAndroidTier())
                    && Utils.IgnoredInteractionsByBasicAndroids.Contains(intDef.defName))
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
    }
}