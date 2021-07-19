using System;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace MOARANDROIDS
{
    [HarmonyPatch(typeof(MentalBreaker), "TryDoRandomMoodCausedMentalBreak")]
    public static class MentalBreaker_AnxietyPatch
    {
        /*
         * Prefix used to store if current pawn which the game require a mentalBreak is an android or not, to be later used in a harmony prefixed psychology's method to check if anxiety is allowed or not
         */
        [HarmonyPrefix]
        public static bool Listener(MentalBreaker __instance, ref bool __result)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();

            if (pawn.IsAndroid())
                Utils.MentalBreakerTryDoRandomMoodCausedMentalBreak_lastPawnIsAndroid = true;
            else
                Utils.MentalBreakerTryDoRandomMoodCausedMentalBreak_lastPawnIsAndroid = false;

            return true;
        }
    }
}