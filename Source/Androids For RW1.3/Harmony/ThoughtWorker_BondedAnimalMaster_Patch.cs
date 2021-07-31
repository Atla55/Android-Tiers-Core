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
    internal class ThoughtWorker_BondedAnimalMaster_Patch

    {
        [HarmonyPatch(typeof(ThoughtWorker_BondedAnimalMaster), "CurrentStateInternal")]
        public class CurrentStateInternal_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn p, ref ThoughtState __result)
            {
                //Already disabled => no more processing required
                if (!__result.Active)
                    return;

                if ((p.IsBasicAndroidTier() || (p.story != null && p.story.traits.HasTrait(TraitDefOf.SimpleMindedAndroid))))
                {
                    __result = ThoughtState.Inactive;
                }
            }
        }
    }
}