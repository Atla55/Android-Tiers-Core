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
    internal class ThoughtWorker_Precept_IdeoDiversity_Uniform_Patch
    {
        [HarmonyPatch(typeof(ThoughtWorker_Precept_IdeoDiversity_Uniform), "ShouldHaveThought")]
        public class TW_Precept_IdeoUniform_ShouldHaveThought
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn p, ref ThoughtState __result)
            {
                if (p.Faction == null || !p.IsColonist)
                {
                    __result = false;
                    return false;
                }
                List<Pawn> list = p.Map.mapPawns.SpawnedPawnsInFaction(p.Faction);
                int num = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != p && list[i].RaceProps.Humanlike && !list[i].IsSlave && !list[i].IsQuestLodger() && !list[i].IsBasicAndroidTier())
                    {
                        if (list[i].Ideo != p.Ideo)
                        {
                            Log.Message("=>" + list[i].LabelCap);
                            __result = false;
                            return false;
                        }
                        num++;
                    }
                }

                __result = num > 0;
                return false;
            }
        }
    }
}