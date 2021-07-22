using Verse;
using HarmonyLib;
using RimWorld;
using System;

namespace MOARANDROIDS
{
    internal class Pawn_StyleTracker_Patch
    {
        /*
         * Prevent androids && VX0 surrogates from wanting to change their look
         */
        [HarmonyPatch(typeof(Pawn_StyleTracker), "get_CanDesireLookChange")]
        public class CanDesireLookChange_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ref bool __result, Pawn ___pawn)
            {
                if (___pawn.IsAndroidTier() || Utils.VX0ChipPresent(___pawn))
                {
                    __result = false;
                }
            }
        }
    }
}