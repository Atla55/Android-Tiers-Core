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
    internal class CompPowerTrader_Patch
    {
        [HarmonyPatch(typeof(CompPowerTrader), "SetUpPowerVars")]
        public class SetUpPowerVars_Patch
        {
            [HarmonyPostfix]
            public static void Listener(CompPowerTrader __instance, ref float ___powerOutputInt)
            {
                CompReloadStation rs = __instance.parent.TryGetComp<CompReloadStation>();
                if (rs != null)
                {
                    rs.refreshPowerConsumed();
                }
            }
        }
    }
}