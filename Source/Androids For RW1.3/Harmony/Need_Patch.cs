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
        private static string energyTranslated = "ATPP_EnergyNeed".Translate();


        private static Pawn get_LabelCapPrevPawn;
        private static bool get_LabelCapPrevIsAndroidTier;

        [HarmonyPatch(typeof(Need), "get_LabelCap")]
        public class get_LabelCap
        {
            [HarmonyPostfix]
            public static void Listener( ref string __result, Pawn ___pawn, Need __instance)
            {
                if (__instance.def.defName == "Food")
                {
                    if (get_LabelCapPrevPawn != ___pawn)
                    {
                        get_LabelCapPrevPawn = ___pawn;
                        get_LabelCapPrevIsAndroidTier = ___pawn.IsAndroidTier();
                    }

                    if (get_LabelCapPrevIsAndroidTier)
                    {
                        __result = energyTranslated;
                    }
                }
            }
        }

        private static Pawn GetTipStringPrevPawn;
        private static bool GetTipStringPrevIsAndroidTier;

        [HarmonyPatch(typeof(Need_Food), "GetTipString")]
        public class GetTipString_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ref string __result, Pawn ___pawn, Need __instance)
            {
                if (__instance.def.defName == "Food")
                {
                    if (GetTipStringPrevPawn != ___pawn)
                    {
                        GetTipStringPrevPawn = ___pawn;
                        GetTipStringPrevIsAndroidTier = ___pawn.IsAndroidTier();
                    }

                    if (GetTipStringPrevIsAndroidTier)
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
}