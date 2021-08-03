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
    internal class ThoughtWorker_Precept_HasProsthetic_Patch
    {
        [HarmonyPatch(typeof(ThoughtWorker_Precept_HasProsthetic), "HasProsthetic")]
        public class TW_Precept_HasProsthetic_HasProsthetic
        {
            [HarmonyPostfix]
            public static void Listener(Pawn p, ref bool __result)
            {
                //Already disabled => no more processing required
                if (__result)
                    return;

                if (p.RaceProps.FleshType == FleshTypeDefOfAT.AndroidTier || p.IsSurrogateAndroid(false, true))
                {
                    __result = true;
                }
            }
        }
    }
}