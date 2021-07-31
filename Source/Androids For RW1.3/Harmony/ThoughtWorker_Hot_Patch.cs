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
    internal class ThoughtWorker_Hot_Patch
    {

        [HarmonyPatch(typeof(ThoughtWorker_Hot), "CurrentStateInternal")]
        public class CurrentStateInternal_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn p, ref ThoughtState __result)
            {
                //Already disabled => no more processing required
                if (!__result.Active)
                    return;

                if (p.IsAndroidTier() || Utils.pawnCurrentlyControlRemoteSurrogate(p) || p.IsSurrogateAndroid(false, true))
                {
                    __result = ThoughtState.Inactive;
                }
            }
        }
    }
}*/