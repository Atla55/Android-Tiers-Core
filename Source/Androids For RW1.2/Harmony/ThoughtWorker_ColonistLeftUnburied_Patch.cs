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
    internal class ThoughtWorker_ColonistLeftUnburied_Patch

    {
        /*
         * PostFix évitant d'attribuer de need comfort et outdoor aux T1 et T2 et l'hygiene a l'ensemble des robots
         */
        [HarmonyPatch(typeof(ThoughtWorker_ColonistLeftUnburied), "CurrentStateInternal")]
        public class CurrentStateInternal_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn p, ref ThoughtState __result)
            {
                if ((Utils.ExceptionAndroidListBasic.Contains(p.def.defName)) || Utils.pawnCurrentlyControlRemoteSurrogate(p) || (p.story != null && p.story.traits.HasTrait(Utils.traitSimpleMinded)))
                {
                    __result = ThoughtState.Inactive;
                }
            }
        }
    }
}