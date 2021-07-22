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
    internal class Thought_IdeoDisrespectedBuilding_Patch

    {
        [HarmonyPatch(typeof(Thought_IdeoDisrespectedBuilding), "CurrentStateInternal")]
        public class CurrentStateInternal_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn ___pawn, ref ThoughtState __result)
            {
                if ((Utils.ExceptionAndroidListBasic.Contains(___pawn.def.defName) || (___pawn.story != null && ___pawn.story.traits.HasTrait(TraitDefOf.SimpleMindedAndroid))))
                {
                    __result = ThoughtState.Inactive;
                }
            }
        }
    }
}