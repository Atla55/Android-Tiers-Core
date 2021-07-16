using System.Collections.Generic;
using System.Linq;
using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MOARANDROIDS
{
    internal static class MapPawns_Patch
    {
        [HarmonyPatch(typeof(MapPawns), "get_AnyPawnBlockingMapRemoval")]
        public class MapPawns_get_AnyPawnBlockingMapRemoval
        {
            [HarmonyPostfix]
            public static void Listener(MapPawns __instance, ref bool __result, List<Pawn> ___pawnsSpawned)
            {
                //Si retour pas true alors check s'il y a de la correction a faire
                if (!__result)
                {
                    Faction ofPlayer = Faction.OfPlayer;
                    for (int i = 0; i < ___pawnsSpawned.Count; i++)
                    {
                        if (___pawnsSpawned[i] == null)
                            continue;

                        CompAndroidState cas = ___pawnsSpawned[i].TryGetComp<CompAndroidState>();

                        //Si pawn non décédé mais est un surrogate inactif
                        if (!___pawnsSpawned[i].Dead && ___pawnsSpawned[i].Faction != null && ___pawnsSpawned[i].Faction.IsPlayer && cas != null && cas.isSurrogate && cas.externalController == null)
                        {
                            __result = true;
                            return;
                        }
                    }
                }
            }
        }

        /*
         * Prefix permetant de jerter en fonction de la config les surrogates des listings
         */
        [HarmonyPatch(typeof(MapPawns), "get_FreeColonists")]
        public class get_FreeColonists_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(ref List<Pawn> __result, MapPawns __instance, Dictionary<Faction, List<Pawn>> ___freeHumanlikesOfFactionResult)
            {
                try
                {
                    if (!Settings.hideInactiveSurrogates)
                        return true;

                    List<Pawn> list;
                    if (!___freeHumanlikesOfFactionResult.TryGetValue(Faction.OfPlayer, out list))
                    {
                        list = new List<Pawn>();
                        ___freeHumanlikesOfFactionResult.Add(Faction.OfPlayer, list);
                    }
                    list.Clear();
                    List<Pawn> allPawns = __instance.AllPawns;
                    for (int i = 0; i < allPawns.Count; i++)
                    {
                        if (allPawns[i].Faction == Faction.OfPlayer && allPawns[i].HostFaction == null && allPawns[i].RaceProps.Humanlike && !allPawns[i].IsSurrogateAndroid(false, true))
                        {
                            list.Add(allPawns[i]);
                        }
                    }

                    __result = list;

                    return false;
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] MapPawns.get_FreeColonists " + e.Message + " " + e.StackTrace);

                    return true;
                }
            }
        }
    }
}