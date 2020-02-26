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
    internal class SickPawnVisitUtility_Patch

    {
        /*
         * PostFix permettant d'éviter que des medecins viennes visiter des pateint T1/T2/M7
         */
        [HarmonyPatch(typeof(SickPawnVisitUtility), "CanVisit")]
        public class CanVisit_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn pawn, Pawn sick, JoyCategory maxPatientJoy, ref bool __result)
            {
                if (sick.IsBasicAndroidTier())
                {
                    __result = false;
                }
            }
        }
    }
}