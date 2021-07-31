using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace MOARANDROIDS
{
    /*
     * Prevent broky UI when trying to convert a basic android
     */
    internal class CompAbilityEffect_Convert_Patch
    {
        [HarmonyPatch(typeof(CompAbilityEffect_Convert), "ExtraLabelMouseAttachment")]
        public class CompAbilityEffect_Convert_ExtraLabelMouseAttachment_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(LocalTargetInfo target, ref string __result)
            {
                if(target != null && target.Pawn != null && target.Pawn.IsBasicAndroidTier())
                {
                    __result = null;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(CompAbilityEffect_Convert), "Valid")]
        public class CompAbilityEffect_Convert_Valid_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(LocalTargetInfo target, ref bool __result)
            {
                if (target != null && target.Pawn != null && target.Pawn.IsBasicAndroidTier())
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(CompAbilityEffect_Convert), "Apply")]
        public class CompAbilityEffect_Convert_Apply_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(LocalTargetInfo target, LocalTargetInfo dest)
            {
                if (target != null && target.Pawn != null && target.Pawn.IsBasicAndroidTier())
                {
                    return false;
                }
                return true;
            }
        }
    }
}