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
        [HarmonyPatch(typeof(Pawn_StyleTracker), "RequestLookChange")]
        public class RequestLookChange_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn ___pawn)
            {
                if (___pawn.IsAndroidTier() || Utils.VX0ChipPresent(___pawn))
                {
                    return false;
                }
                return true;
            }
        }
    }
}