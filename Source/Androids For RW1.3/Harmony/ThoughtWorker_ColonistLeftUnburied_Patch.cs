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
        [HarmonyPatch(typeof(ThoughtWorker_ColonistLeftUnburied), "CurrentStateInternal")]
        public class CurrentStateInternal_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn p, ref ThoughtState __result)
            {
                //Already disabled => no more processing required
                if (!__result.Active)
                    return;

                if (p.IsBasicAndroidTier() || Utils.pawnCurrentlyControlRemoteSurrogate(p) || (p.story != null && p.story.traits.HasTrait(TraitDefOf.SimpleMindedAndroid)))
                {
                    __result = ThoughtState.Inactive;
                }
            }
        }
    }
}