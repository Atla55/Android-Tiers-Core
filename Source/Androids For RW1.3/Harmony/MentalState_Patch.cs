﻿using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MOARANDROIDS
{
    internal class MentalState_Patch

    {
        /*
         * PostFix servant a desactivé les moods liés a la joie pour les T1 et T2
         */
        [HarmonyPatch(typeof(MentalState), "PostEnd")]
        public class PostEnd_Patch
        {
            [HarmonyPostfix]
            public static void Listener(MentalState __instance)
            {
                if (__instance.pawn.IsSurrogateAndroid())
                {
                    CompSkyMind csm = Utils.getCachedCSM(__instance.pawn);
                    if (csm == null)
                        return;

                    if (csm.Infected == 4)
                    {
                        csm.Infected = -1;
                        Hediff he = __instance.pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_NoHost);
                        if (he == null)
                        {
                            __instance.pawn.health.AddHediff(HediffDefOf.ATPP_NoHost);
                        }
                    }
                }
            }
        }

        /*
         * PostFix servant a deconencté de ses activitées un mind ayant un mentalbreak et initié un timeout de fin de break
         */
        [HarmonyPatch(typeof(MentalStateHandler), "TryStartMentalState")]
        public class TryStartMentalState_Patch
        {
            [HarmonyPostfix]
            public static void Listener(MentalStateDef stateDef, string reason, bool forceWake, bool causedByMood, Pawn otherPawn, bool transitionSilently, Pawn ___pawn, MentalStateHandler __instance, ref bool __result)
            {
                if (__result && ___pawn.IsSurrogateAndroid())
                {
                    CompAndroidState cas = Utils.getCachedCAS(___pawn);

                    if (cas == null || cas.surrogateController == null)
                        return;

                    CompSurrogateOwner cso = Utils.getCachedCSO(cas.surrogateController);
                    if(cso.skyCloudHost != null)
                    {
                        CompSkyCloudCore csc = Utils.getCachedCSC(cso.skyCloudHost);
                        if (csc == null)
                            return;


                        //Ajout a une liste de minds boudant avec timeout
                        csc.setMentalBreak(cas.surrogateController);
                    }
                }
            }
        }

    }
}