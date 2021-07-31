﻿using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MOARANDROIDS
{
    internal class RaceProperties_Patch

    {
        /*
         * PostFix évitant d'attribuer de need comfort et outdoor aux T1 et T2 et l'hygiene a l'ensemble des robots (gestationPeriodDays est utilisé en amont pour encoder s'il sagit d'un robot avancé ou non)
         */
        [HarmonyPatch(typeof(RaceProperties), "CanEverEat")]
        [HarmonyPatch(new Type[] { typeof(ThingDef) })]
        public class CanEverEat_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ThingDef t, ref bool __result, FleshTypeDef ___fleshType, float ___gestationPeriodDays)
            {
                //Prevent extra-processing
                if (!__result)
                    return;

                if (___fleshType != FleshTypeDefOfAT.AndroidTier)
                    return;

                //Si inside insideAddHumanlikeOrders et que param activé alors masquage capacité a forcer manger living plants
                if (Utils.insideAddHumanlikeOrders && Settings.androidsCanConsumeLivingPlants && Settings.hideMenuAllowingForceEatingLivingPlants)
                {
                    if ( t != null  && t.plant != null)
                    {
                        __result = false;
                    }
                }


                if (Settings.allowHumanDrugsForAndroids)
                    return;

                //Si android alors on desactive les drogues humaines
                if( t != null && Utils.BlacklistAndroidFood.Contains(t.defName) && !(Settings.allowHumanDrugsForT3PlusAndroids && ___gestationPeriodDays == 2))
                {
                    __result = false;
                }
            }
        }
    }
}