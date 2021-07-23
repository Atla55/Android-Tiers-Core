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
    internal class Precept_Ritual_Patch

    {
        [HarmonyPatch(typeof(Precept_Ritual), "AddObligation")]
        public class AddObligation_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(RitualObligation obligation)
            {
                try
                {
                    if (obligation.targetA.Thing != null
                        && ((obligation.targetA.Thing is Pawn && ((Pawn)obligation.targetA.Thing).IsBasicAndroidTier())
                             || (obligation.targetA.Thing is Corpse && ((Corpse)obligation.targetA.Thing).InnerPawn.IsBasicAndroidTier())))
                        return false;
                    else
                        return true;
                }
                catch (Exception e)
                {
                    Log.Message("[ATPP] Precept_Ritual.AddObligation : " + e.Message + " - " + e.StackTrace);
                    return true;
                }
            }
        }

    }
}