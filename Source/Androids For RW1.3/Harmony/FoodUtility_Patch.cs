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
    internal class WildManUtility_Patch

    {


        [HarmonyPatch(typeof(FoodUtility), "BestFoodSourceOnMap")]
        public class BestFoodSourceOnMap_Patch
        {
            /*
             * Si option comme quoi android peut manger living plantes activés alors on va le faire passer en non humanlike pour que sa soit le code relatif aux animaux herbivores qui soit appliqué et non celui au humains (ce qui risque de faire lagguer aussinon)
             */
            [HarmonyPrefix]
            public static bool Listener1(Pawn getter, Pawn eater, bool desperate, ThingDef foodDef, FoodPreferability maxPref = FoodPreferability.MealLavish, bool allowPlant = true, bool allowDrug = true, bool allowCorpse = true, bool allowDispenserFull = true, bool allowDispenserEmpty = true, bool allowForbidden = false, bool allowSociallyImproper = false, bool allowHarvest = false, bool forceScanWholeMap = false)
            {
                if (Settings.androidsCanConsumeLivingPlants && eater != null && eater.IsAndroidTier() && eater == getter)
                {
                    getter.RaceProps.intelligence = Intelligence.Animal;
                }

                return true;
            }

            [HarmonyPostfix]
            public static void Listener2(Pawn getter, Pawn eater, bool desperate, ThingDef foodDef, FoodPreferability maxPref = FoodPreferability.MealLavish, bool allowPlant = true, bool allowDrug = true, bool allowCorpse = true, bool allowDispenserFull = true, bool allowDispenserEmpty = true, bool allowForbidden = false, bool allowSociallyImproper = false, bool allowHarvest = false, bool forceScanWholeMap = false)
            {
                if (Settings.androidsCanConsumeLivingPlants && eater != null &&  eater.IsAndroidTier() && eater == getter)
                {
                    getter.RaceProps.intelligence = Intelligence.Humanlike;
                }

            }
        }

    }
}