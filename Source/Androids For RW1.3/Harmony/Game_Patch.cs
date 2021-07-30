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
    internal class Game_Patch

    {
        [HarmonyPatch(typeof(Game), "InitNewGame")]
        public class Game_InitNewGame_Patch
        {
            [HarmonyPrefix]
            public static void Listener()
            {
                try
                {
                    Utils.GCATPP.reset();
                }
                catch (Exception)
                {

                }
            }
        }

        [HarmonyPatch(typeof(Game), "LoadGame")]
        public class Game_LoadGame_Patch
        {
            [HarmonyPrefix]
            public static void Listener()
            {
                try
                {
                    Utils.GCATPP.reset();
                }
                catch (Exception)
                {

                }
            }
        }

    }
}