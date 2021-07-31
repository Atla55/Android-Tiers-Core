﻿using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace MOARANDROIDS
{
    /*
     * Prevent basic android corpse to be reported as unburied
     */
    internal class Alert_ColonistLeftUnburied_Patch
    {
        [HarmonyPatch(typeof(Alert_ColonistLeftUnburied), "IsCorpseOfColonist")]
        public class Alert_ColonistLeftUnburied_IsCorpseOfColonist_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Corpse corpse, ref bool __result)
            {
                //No need for extra processing
                if (!__result)
                    return;

                Pawn p = corpse.InnerPawn;
                if (p != null && p.IsBasicAndroidTier() || Utils.pawnCurrentlyControlRemoteSurrogate(p) || (p.story != null && p.story.traits.HasTrait(TraitDefOf.SimpleMindedAndroid)))
                    __result = false;
 
            }
        }
    }
}