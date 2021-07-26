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
    internal class Thought_IdeoRoleEmpty_Patch

    {
        [HarmonyPatch(typeof(Thought_IdeoRoleEmpty), "CurrentStateInternal")]
        public class CurrentStateInternal_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn ___pawn, ref ThoughtState __result)
            {
                if ((___pawn.IsBasicAndroidTier() || (___pawn.story != null && ___pawn.story.traits.HasTrait(TraitDefOf.SimpleMindedAndroid))))
                {
                    __result = ThoughtState.Inactive;
                }
            }
        }
    }
}