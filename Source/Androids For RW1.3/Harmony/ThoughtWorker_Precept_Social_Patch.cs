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
    internal class ThoughtWorker_Precept_Social_Patch

    {
        [HarmonyPatch(typeof(ThoughtWorker_Precept_Social), "CurrentSocialStateInternal")]
        public class TW_Precept_Social_CurrentSocialStateInternal
        {

            [HarmonyPostfix]
            public static void Listener(Pawn p, Pawn otherPawn, ref ThoughtState __result)
            {
                //Already disabled => no more processing required
                if (!__result.Active)
                    return;

                if (p.IsBasicAndroidTier() || otherPawn.IsBasicAndroidTier() || (p.IsSurrogateAndroid(false, true) || otherPawn.IsSurrogateAndroid(false, true)))
                {
                    __result = ThoughtState.Inactive;
                }
            }
        }
    }
}