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
    internal class Recipe_InstallImplant_Patch

    {

        /*
         * PreFix 
         */
        [HarmonyPatch(typeof(Recipe_InstallImplant), "ApplyOnPawn")]
        public class ApplyOnPawnPrefix_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
            {
                Utils.lastInstallImplantBillDoer = billDoer;
                return true;
            }
        }

        /*
         * PostFix 
         */
        [HarmonyPatch(typeof(Recipe_InstallImplant), "ApplyOnPawn")]
        public class CurrentStateInternalPostfix_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
            {
                Utils.lastInstallImplantBillDoer = null;
            }
        }
    }
}