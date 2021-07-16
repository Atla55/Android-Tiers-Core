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
    internal class ThingDef_Patch

    {
        /*
         * PostFix permettant Si androide surrogate on va virer la possibilité d'ajouter des VX chips
         */
        [HarmonyPatch(typeof(RecipeDef), "get_AvailableNow")]
        public class get_AvailableNow_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ref bool __result, RecipeDef __instance)
            {
                if (Utils.curSelPatientDrawMedOperationsTab != null)
                {
                    if((Utils.curSelPatientDrawMedOperationsTab is Pawn))
                    {
                        CompAndroidState cas = Utils.curSelPatientDrawMedOperationsTab.TryGetComp<CompAndroidState>();
                        
                        if(cas != null)
                        {
                            //Si androide surrogate on va virer la possibilité d'ajouter des VX chips
                            if (cas.isSurrogate)
                            {
                                if(Utils.ExceptionVXNeuralChipSurgery.Contains(__instance.defName))
                                    __result = false;
                            }
                            else
                            {
                                //Sinon si pas surrogate et que le recipe en question est à destination des surrogates on le squeeze
                                if (Utils.ExceptionArtificialBrainsSurgery.Contains(__instance.defName))
                                    __result = false;
                            }
                        }
                    }
                }
            }
        }
    }
}