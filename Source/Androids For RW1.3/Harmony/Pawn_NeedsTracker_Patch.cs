﻿using Verse;
using HarmonyLib;
using RimWorld;
using System;

namespace MOARANDROIDS
{
    internal class Pawn_NeedsTracker_Patch
    {
        /*
         * PostFix évitant d'attribuer de need comfort et outdoor aux T1 et T2 et l'hygiene a l'ensemble des robots
         */
        [HarmonyPatch(typeof(Pawn_NeedsTracker), "ShouldHaveNeed")]
        public class ShouldHaveNeed_Patch
        {
            [HarmonyPostfix]
            public static void Listener(NeedDef nd, ref bool __result, Pawn ___pawn)
            {
                try
                {
                    bool isAndroid = ___pawn.IsAndroidTier();

                    //SI pas un androide on jerte
                    if (!isAndroid)
                        return;

                    bool advancedAndroids = ___pawn.IsAdvancedAndroidTier();

                    if ((___pawn.IsBasicAndroidTier()
                        && (nd.defName == "Outdoors" ))
                        || (___pawn.def.defName == "Android1Tier" && nd.defName == "Beauty")
                        || (isAndroid && (nd.defName == "Hygiene" || nd.defName == "Bladder" || nd.defName ==  "DBHThirst"))
                        || (nd.defName == "Comfort" && (!advancedAndroids || (advancedAndroids && Settings.removeComfortNeedForT3T4))))
                    {
                        __result = false;
                    }

                    //Activation besoin de bouffe pour les M7 surrogates (SM7)
                    if (___pawn.def.defName == "M7Mech" && ___pawn.IsSurrogateAndroid() && nd.defName == "Food")
                        __result = true;
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] Pawn_StoryTracker.ShouldHaveNeed : " + e.Message + " - " + e.StackTrace);
                }
            }
        }


        /*[HarmonyPatch(typeof(Pawn_NeedsTracker), "NeedsTrackerTick")]
        public class NeedsTrackerTick_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn ___pawn)
            {
                CompSurrogateOwner cso = ___pawn.TryGetComp<CompSurrogateOwner>();
                if (cso != null && cso.skyCloudHost != null)
                {
                    Log.Message("Pawn_NeedsTracker " + ___pawn.LabelCap);
                }
            }
        }*/
    }
}