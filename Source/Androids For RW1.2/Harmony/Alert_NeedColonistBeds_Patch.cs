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
    internal class Alert_NeedColonistBeds_Patch
    {
        [HarmonyPatch(typeof(Alert_NeedColonistBeds), "NeedColonistBeds")]
        public class NeedColonistBeds_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Map map, ref bool __result)
            {
                try
                {
                    if (!map.IsPlayerHome)
                    {
                        return false;
                    }
                    int num = 0;
                    int num2 = 0;
                    List<Building> allBuildingsColonist = map.listerBuildings.allBuildingsColonist;
                    for (int i = 0; i < allBuildingsColonist.Count; i++)
                    {
                        Building_Bed building_Bed = allBuildingsColonist[i] as Building_Bed;
                        if (building_Bed != null && !building_Bed.ForPrisoners && !building_Bed.Medical && building_Bed.def.building.bed_humanlike)
                        {
                            if (building_Bed.SleepingSlotsCount == 1)
                            {
                                num++;
                            }
                            else
                            {
                                num2++;
                            }
                        }
                    }
                    int num3 = 0;
                    int num4 = 0;
                    foreach (Pawn current in map.mapPawns.FreeColonistsSpawned)
                    {
                        //On ignore les androis dans la comptabilisation
                        if (Utils.ExceptionAndroidList.Contains(current.def.defName))
                            continue;

                        Pawn pawn = LovePartnerRelationUtility.ExistingMostLikedLovePartner(current, false);
                        if (pawn == null || !pawn.Spawned || pawn.Map != current.Map || pawn.Faction != Faction.OfPlayer || pawn.HostFaction != null)
                        {
                            num3++;
                        }
                        else
                        {
                            num4++;
                        }
                    }
                    if (num4 % 2 != 0)
                    {
                        //Log.ErrorOnce("partneredCols % 2 != 0", 743211, false);
                    }
                    for (int j = 0; j < num4 / 2; j++)
                    {
                        if (num2 > 0)
                        {
                            num2--;
                        }
                        else
                        {
                            num -= 2;
                        }
                    }
                    for (int k = 0; k < num3; k++)
                    {
                        if (num2 > 0)
                        {
                            num2--;
                        }
                        else
                        {
                            num--;
                        }
                    }
                    __result = num < 0 || num2 < 0;

                    return false;
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] ALert_NeedColonistBeds.NeedColonistBeds :" + e.Message + " - " + e.StackTrace);
                    return true;
                }
            }
        }
    }
}