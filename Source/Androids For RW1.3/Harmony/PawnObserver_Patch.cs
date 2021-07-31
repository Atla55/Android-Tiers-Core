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
    internal class PawnObserver_Patch
    {
        /*
         * Prevent observed dead androids body memories
         */
        [HarmonyPatch(typeof(PawnObserver), "PossibleToObserve")]
        public class PawnObserver_PossibleToObserve_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Thing thing, ref bool __result)
            {
                if (!__result)
                    return;

                if(thing is Corpse)
                {
                    Corpse cp = (Corpse)thing;
                    if (cp.InnerPawn.IsAndroidTier())
                    {
                        __result = false;
                    }
                }
            }
        }
    }
}