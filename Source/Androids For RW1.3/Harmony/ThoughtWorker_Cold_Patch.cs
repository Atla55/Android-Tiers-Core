/*using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MOARANDROIDS
{
    internal class ThoughtWorker_Cold_Patch

    {
        [HarmonyPatch(typeof(ThoughtWorker_Cold), "CurrentStateInternal")]
        public class CurrentStateInternal_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn p, ref ThoughtState __result)
            {
                //Already disabled => no more processing required
                if (!__result.Active)
                    return;

                if (Utils.IsBasicAndroidTier(p) || Utils.pawnCurrentlyControlRemoteSurrogate(p) || p.IsSurrogateAndroid(false, true))
                {
                    __result = ThoughtState.Inactive;
                }
            }
        }
    }
}*/