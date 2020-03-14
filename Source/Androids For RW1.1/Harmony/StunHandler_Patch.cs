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
    internal class StunHandler_Patch

    {
        /*
         * Allow android tiers to be affected by EMP
         */
        [HarmonyPatch(new Type[] { typeof(DamageInfo), typeof(bool) })]
        [HarmonyPatch(typeof(StunHandler), "Notify_DamageApplied")]
        public class Notify_DamageApplied_Patch
        {
            [HarmonyPrefix]
            public static void Listener(DamageInfo dinfo, ref bool affectedByEMP, Thing ___parent )
            {
                if(___parent is Pawn)
                {
                    Pawn pawn = (Pawn)___parent;
                    if ((Utils.ExceptionAndroidWithoutSkinList.Contains(pawn.def.defName) ) || Utils.ExceptionAndroidAnimals.Contains(pawn.def.defName))
                    {
                        affectedByEMP = true;
                    }
                }
            }
        }
    }
}