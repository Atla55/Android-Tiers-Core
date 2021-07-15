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
    internal class ThoughtWorker_Dark_Patch

    {
        [HarmonyPatch(typeof(ThoughtWorker_Dark), "CurrentStateInternal")]
        public class CurrentStateInternal_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn p, ref ThoughtState __result)
            {
                if ((Utils.ExceptionAndroidListBasic.Contains(p.def.defName) || (p.story != null && p.story.traits.HasTrait(Utils.traitSimpleMinded))))
                {
                    __result = ThoughtState.Inactive;
                }
            }
        }
    }
}