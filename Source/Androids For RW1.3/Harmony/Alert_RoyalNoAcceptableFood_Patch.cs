using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace MOARANDROIDS
{
    /*
     * Remove androids from Alert_RoyalNoAcceptableFood, as androids with title dont care about food quality
     */
    internal class Alert_RoyalNoAcceptableFood_Patch
    {
        [HarmonyPatch(typeof(Alert_RoyalNoAcceptableFood), "get_Targets")]
        public class Alert_RoyalNoAcceptableFood_get_Targets_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ref List<Pawn> __result)
            {
                for(int i = __result.Count-1;i >= 0; i--)
                {
                    if (__result[i].RaceProps.FleshType == FleshTypeDefOfAT.AndroidTier)
                        __result.RemoveAt(i);
                }
            }
        }
    }
}