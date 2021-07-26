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
    internal class Need_Patch
    {
        private static string energyTranslated;

        [HarmonyPatch(typeof(Need), "get_LabelCap")]
        public class get_LabelCap
        {
            [HarmonyPostfix]
            public static void Listener( ref string __result, Pawn ___pawn, Need __instance)
            {
                if (__instance.def.defName == "Food" && ___pawn.IsAndroidTier())
                {
                    if(energyTranslated == null)
                        energyTranslated = "ATPP_EnergyNeed".Translate();

                    __result = energyTranslated;
                }
            }
        }

        [HarmonyPatch(typeof(Need_Food), "GetTipString")]
        public class GetTipString_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ref string __result, Pawn ___pawn, Need __instance)
            {
                if (__instance.def.defName == "Food" && ___pawn.IsAndroidTier())
                {
                    __result = string.Concat(new string[]
                        {
                            __instance.LabelCap,
                            ": ",
                            __instance.CurLevelPercentage.ToStringPercent(),
                            " (",
                            __instance.CurLevel.ToString("0.##"),
                            " / ",
                            __instance.MaxLevel.ToString("0.##"),
                            ")\n",
                            "ATPP_EnergyNeedDesc".Translate()
                        });
                }
            }
        }
    }
}