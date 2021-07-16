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
    internal class Apparel_Patch

    {
        /*
         * Virer le Tainted quand android portant un vetement est tué
         */
        [HarmonyPatch(typeof(Apparel), "Notify_PawnKilled")]
        public class Notify_PawnKilled_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Apparel __instance, ref bool ___wornByCorpseInt)
            {
                Pawn p = __instance.Wearer;

                if (p != null && p.IsAndroidTier())
                {
                    ___wornByCorpseInt = false;
                }
            }
        }
    }
}