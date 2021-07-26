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
    internal class Corpse_Patch
    {
        /*
         * PostFix servant à annuler les moods du butchering d'un android de Android Tiers
         */
        [HarmonyPatch(typeof(Corpse), "ButcherProducts")]
        public class ButcherProducts_Patch                   
        {
            [HarmonyPostfix]
            public static void Listener(Pawn butcher, float efficiency, Corpse __instance, ref IEnumerable<Thing> __result)
            {
                Utils.lastButcheredPawnIsAndroid = false;

                CompAndroidState cas = Utils.getCachedCAS(__instance.InnerPawn);
                //Si Surrogate T4 butcherisé alors on supprime le IA-Core des produits 
                if (__instance.InnerPawn != null && __instance.InnerPawn.def.defName == Utils.T4 && cas != null && __result != null)
                {
                    if (cas.isSurrogate)
                    {
                        List<Thing> res = new List<Thing>();
                        foreach(var r in __result.ToList())
                        {
                            if (r.def != null && r.def.defName != "AIPersonaCore")
                                res.Add(r);
                        }
                        __result = res;
                    }
                }

            }
        }


        /*
         * PostFix servant à annuler les moods de voir le corp d'un android de Android Tiers mort ou pourrie
         */
        [HarmonyPatch(typeof(Corpse), "GiveObservedThought")]
        public class GiveObservedThought_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Corpse __instance, ref Thought_Memory __result)
            {
                if (__instance.InnerPawn.RaceProps.Humanlike
                    && __instance.InnerPawn.IsAndroidTier())
                {
                    __result = null;
                }
            }
        }

        /*
         * PostFix servant à annuler la possibilité de manger des androids
         */
        [HarmonyPatch(typeof(Corpse), "get_IngestibleNow")]
        public class IngestibleNow_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Corpse __instance, ref bool __result)
            {
                if ((__instance.InnerPawn.RaceProps.Humanlike
                    && __instance.InnerPawn.IsAndroidTier())
                    || __instance.InnerPawn.IsCyberAnimal())
                {
                        __result = false;
                }
            }
        }

    }
}