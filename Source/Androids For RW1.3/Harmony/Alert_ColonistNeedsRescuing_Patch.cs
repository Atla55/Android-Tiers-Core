using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace MOARANDROIDS
{
    internal class Alert_ColonistNeedsRescuing_Patch
    {
        [HarmonyPatch(typeof(Alert_ColonistNeedsRescuing), "NeedsRescue")]
        public class ColonistsNeedingRescue_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn p, ref bool __result)
            {
                __result = __result && !p.IsSurrogateAndroid();
 
            }
        }
    }
}