using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

namespace MOARANDROIDS
{
    internal class ApparelUtility_Patch

    {
        [HarmonyPatch(typeof(ApparelUtility), "CanWearTogether")]
        public class CanWearTogether_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ThingDef A, ThingDef B, BodyDef body, ref bool __result)
            {
                if(A.defName == "VAE_Headgear_Scarf" && B.defName == "VAE_Headgear_Scarf")
                    __result = false;
            }
        }

        [HarmonyPatch(typeof(ApparelUtility), "HasPartsToWear")]
        public class HasPartsToWear_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn p, ThingDef apparel, ref bool __result)
            {
                try
                {
                    if (!p.IsAndroidTier())
                        return;

                    bool councernFeet = apparel.apparel.bodyPartGroups.Contains(DefDatabase<BodyPartGroupDef>.GetNamed("Feet",false));
                    bool councernHanbd = apparel.apparel.bodyPartGroups.Contains(DefDatabase<BodyPartGroupDef>.GetNamed("Hands", false));
                    if (councernHanbd || councernFeet)
                    {
                        if (councernFeet)
                        {
                            foreach (var el in p.health.hediffSet.hediffs)
                            {
                                if ( Utils.ExceptionBionicHaveFeet.Contains(el.def.defName))
                                {
                                    __result = true;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            foreach (var el in p.health.hediffSet.hediffs)
                            {
                                if (Utils.ExceptionBionicHaveHand.Contains(el.def.defName))
                                {
                                    __result = true;
                                    return;
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Log.Message("[ATPP] ApparelUtility.HasPartsToWear " + ex.Message + " " + ex.StackTrace);
                }
            }
        }
    }
}