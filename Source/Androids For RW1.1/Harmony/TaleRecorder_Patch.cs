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
    internal class TaleRecorder_Patch

    {
        [HarmonyPatch(typeof(TaleRecorder), "RecordTale")]
        public class CurrentStateInternal_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(TaleDef def, params object[] args)
            {
                Pawn p1 = null;
                Pawn p2 = null;
                int nba = args.Count();

                if (nba >= 2)
                {
                    p1 = args[0] as Pawn;
                    p2 = args[1] as Pawn;
                }
                else if (nba == 1)
                    p1 = args[0] as Pawn;
                else
                    return true;

                //Si colon tué est un T1/T2 OU un surrogate on s'en fou
                if (def == TaleDefOf.KilledColonist)
                {
                    if ((p2 != null && (Utils.ExceptionAndroidListBasic.Contains(p2.def.defName) || (p2.TryGetComp<CompAndroidState>() != null && p2.TryGetComp<CompAndroidState>().isSurrogate))))
                        return false;
                }
                //Si androide butcherisé
                else if (def == TaleDefOf.ButcheredHumanlikeCorpse)
                {
                    if (Utils.lastButcheredPawnIsAndroid)
                        return false;
                }
                //SI viande humaine mangée par androide
                else if (def == TaleDefOf.AteRawHumanlikeMeat)
                {
                    if (p1 != null && Utils.ExceptionAndroidList.Contains(p1.def.defName))
                        return false;
                }

                return true;
            }
        }
    }
}