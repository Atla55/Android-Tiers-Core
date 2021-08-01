using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace MOARANDROIDS
{
    internal class ThoughtWorker_Precept_IdeoDiversity_Patch
    {
        [HarmonyPatch(typeof(ThoughtWorker_Precept_IdeoDiversity), "ShouldHaveThought")]
        public class TW_Precept_IdeoDiversity_ShouldHaveThought
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn p, ref ThoughtState __result, ThoughtWorker_Precept_IdeoDiversity __instance)
            {
                if (p.Faction == null || !p.IsColonist)
                {
                    __result = false;
                    return false;
                }
                int num = 0;
                int num2 = 0;
                List<Pawn> list = p.Map.mapPawns.SpawnedPawnsInFaction(p.Faction);
                for (int i = 0; i < list.Count; i++)
                {
                    if (!list[i].IsQuestLodger() && list[i].RaceProps.Humanlike && !list[i].IsSlave && !list[i].IsPrisoner && !list[i].IsBasicAndroidTier())
                    {
                        num2++;
                        if (list[i] != p && list[i].Ideo != p.Ideo)
                        {
                            num++;
                        }
                    }
                }
                if (num == 0)
                {
                    __result = ThoughtState.Inactive;
                    return false;
                }
                __result = ThoughtState.ActiveAtStage(Mathf.RoundToInt((float)num / (float)(num2 - 1) * (float)(__instance.def.stages.Count - 1)));
                return false;
            }
        }
    }
}