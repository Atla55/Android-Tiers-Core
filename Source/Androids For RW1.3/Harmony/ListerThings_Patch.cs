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
    internal class ListerThings_Patch

    {
        /*
         * QEE patch permettant d'obtenir les GeneticsSequencers custom de AT++ pour une recherche de GeneticSequencer de base
         */
        [HarmonyPatch(typeof(ListerThings), "ThingsMatching")]
        public class ThingsMatching_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ListerThings __instance, ThingRequest req, ref List<Thing> __result)
            {
                List<Thing> ret = null;
                if (Utils.QEE_LOADED && req.singleDef != null && req.singleDef.defName  == "QE_GenomeSequencerFilled")
                {
                    foreach (var el in __instance.AllThings)
                    {
                        if(el.def.defName == "QE_GenomeSequencerFilled" || Utils.ExceptionQEEGS.Contains(el.def.defName))
                        {
                            if (ret == null)
                            {
                                ret = new List<Thing>();
                            }
                            ret.Add(el);
                        }
                    }
                    __result = ret;
                }
            }
        }
    }
}