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
    internal class ColonistBar_Patch

    {
        [HarmonyPatch(typeof(ColonistBar), "CheckRecacheEntries")]
        public class CheckRecacheEntries_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ColonistBar __instance, ref List<ColonistBar.Entry> ___cachedEntries, ref ColonistBarDrawLocsFinder ___drawLocsFinder, ref List<Vector2> ___cachedDrawLocs, ref float ___cachedScale)
            {
                try
                {
                    if (!Settings.hideInactiveSurrogates)
                        return;

                    List<ColonistBar.Entry> toDel = null;
                    //Suppresssion de la barre du haut des surrogates non actifs
                    foreach (var e in ___cachedEntries)
                    {
                        if (e.pawn.IsSurrogateAndroid(false, true))
                        {
                            if (toDel == null)
                                toDel = new List<ColonistBar.Entry>();

                            toDel.Add(e);
                        }
                    }
                    if (toDel != null)
                    {
                        foreach (var e in toDel)
                        {
                            ___cachedEntries.Remove(e);
                        }

                        __instance.drawer.Notify_RecachedEntries();

                        ___drawLocsFinder.CalculateDrawLocs(___cachedDrawLocs, out ___cachedScale);
                    }
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] Colonistbar.CheckRecacheEntries " + e.Message + " " + e.StackTrace);
                }
            }
        }
    }
}