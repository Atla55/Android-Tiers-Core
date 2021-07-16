/*using System.Collections.Generic;
using System.Linq;
using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MOARANDROIDS
{
    internal static class StorytellerComp_Triggered_Patch
    {
        [HarmonyPatch(typeof(StorytellerComp_Triggered), "Notify_PawnEvent")]
        public class Notify_PawnEvent
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn p, AdaptationEvent ev, DamageInfo? dinfo = null)
            {
                if (p == null || p.RaceProps == null ||!p.RaceProps.Humanlike || !p.IsColonist)
                {
                    return false;
                }
                CompAndroidState cas = p.TryGetComp<CompAndroidState>();

                //Si pawn downed ET est un surrogate non controllé alors on ignore afin d'éviter l'evenement stanger in black
                if ((ev == AdaptationEvent.Downed) && cas != null && cas.isSurrogate && cas.surrogateController == null)
                {
                    return false;
                }

                return true;
            }
        }
    }
}*/