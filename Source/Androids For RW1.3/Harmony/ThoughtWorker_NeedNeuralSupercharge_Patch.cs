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
    internal class ThoughtWorker_NeedNeuralSupercharge_Patch

    {
        /*
         * Basic androids dont care about nudity stuff
         */
        [HarmonyPatch(typeof(ThoughtWorker_NeedNeuralSupercharge), "ShouldHaveThought")]
        public class CurrentStateInternal_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn p, ref ThoughtState __result)
            {
                //Already disabled => no more processing required
                if (!__result.Active)
                    return;

                if (p.IsBasicAndroidTier() || p.IsSurrogateAndroid(false, true))
                {
                    __result = ThoughtState.Inactive;
                }
            }
        }
    }
}