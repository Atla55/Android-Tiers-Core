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
    internal class FloatMenuMakerMap_Patch

    {
        [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
        public class AddHumanlikeOrders_Patch
        {
            [HarmonyPrefix]
            public static bool ListenerPrefix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
            {
                Utils.insideAddHumanlikeOrders = true;
                return true;
            }

            [HarmonyPostfix]
            public static void ListenerPostfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
            {
                Utils.insideAddHumanlikeOrders = false;
            }
        }
    }
}