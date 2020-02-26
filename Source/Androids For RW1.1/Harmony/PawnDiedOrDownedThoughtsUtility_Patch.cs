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
    internal class PawnDiedOrDownedThoughtsUtility_Patch
    {
        /*
         * PreFix évitant debuff en cas de mort d'un T1 ou T2
         */
        [HarmonyPatch(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_ForHumanlike")]
        public class AppendThoughts_ForHumanlike_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(ref Pawn victim)
            {
                //Si android avec peu de logique alors tous le monde se fout de sa mort OU si Surrogate
                if (Utils.ExceptionAndroidListBasic.Contains(victim.def.defName) || victim.IsSurrogateAndroid() || victim.IsBlankAndroid())
                    return false;
                else
                    return true;
            }
        }
    }
}