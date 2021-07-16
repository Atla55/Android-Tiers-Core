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
    internal class ThoughtWorker_Expectations_Patch

    {
        /*
         * PostFix servant a desactivé les moods liés a l'expectation pour les T1 et T2
         */
        [HarmonyPatch(typeof(ThoughtWorker_Expectations), "CurrentStateInternal")]
        public class CurrentStateInternal_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn p, ref ThoughtState __result)
            {
                if (p.def.defName == "Android1Tier" || p.def.defName == "M7Mech"  || p.def.defName == Utils.TX2K)
                {
                    __result = ThoughtState.ActiveAtStage(5);
                }
                else if (p.def.defName == Utils.T2 || p.def.defName == Utils.TX2 || (p.story != null && p.story.traits.HasTrait(Utils.traitSimpleMinded)))
                {
                    __result = ThoughtState.ActiveAtStage(4);
                }
            }
        }
    }
}