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
    internal class ThoughtWorker_Precept_Patch

    {
        [HarmonyPatch(typeof(ThoughtWorker_Precept), "CurrentStateInternal")]
        public class CurrentStateInternal_Patch
        {
            static private Pawn lastPawn;
            static private bool lastShouldBeInactive = false;

            [HarmonyPostfix]
            public static void Listener(Pawn p, ref ThoughtState __result)
            {
                //Already disabled => no more processing required
                if (!__result.Active)
                    return;

                //Caching result
                if (p != lastPawn)
                {
                    lastPawn = p;
                    lastShouldBeInactive = p.IsBasicAndroidTier();
                }

                if ( lastShouldBeInactive || p.IsSurrogateAndroid(false, true))
                {
                    __result = ThoughtState.Inactive;
                }
            }
        }
    }
}