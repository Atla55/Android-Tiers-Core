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
             * If option like what android can eat living plants activated then we will make it pass in non humanlike so that its is the code relating to herbivorous animals which is applied and not that to humans (which may cause lagging as well)
             */
            [HarmonyPrefix]
            public static bool Listener1(Pawn getter, Pawn eater, bool desperate, ThingDef foodDef, FoodPreferability maxPref = FoodPreferability.MealLavish, bool allowPlant = true, bool allowDrug = true, bool allowCorpse = true, bool allowDispenserFull = true, bool allowDispenserEmpty = true, bool allowForbidden = false, bool allowSociallyImproper = false, bool allowHarvest = false, bool forceScanWholeMap = false)
            {
                if (Settings.androidsCanConsumeLivingPlants && eater != null && eater.IsAndroidTier() && eater == getter)
                {
                    //Log.Message(desperate + " " + maxPref + " " + allowPlant + " " + allowDrug + " " + allowCorpse + " " + allowDispenserFull + " " + allowDispenserEmpty + " " + allowForbidden + " " + allowSociallyImproper + " " + allowHarvest + " " + forceScanWholeMap);
                    //If not desperate switch on standard way, because otherwise it will cause massive lag
                    if(!desperate)
                    {
                        getter.RaceProps.foodType = FoodTypeFlags.OmnivoreHuman;
                    }
                    else
                        getter.RaceProps.intelligence = Intelligence.Animal;
                }

                return true;
            }

            [HarmonyPostfix]
            public static void Listener2(Pawn getter, Pawn eater, bool desperate, ThingDef foodDef, FoodPreferability maxPref = FoodPreferability.MealLavish, bool allowPlant = true, bool allowDrug = true, bool allowCorpse = true, bool allowDispenserFull = true, bool allowDispenserEmpty = true, bool allowForbidden = false, bool allowSociallyImproper = false, bool allowHarvest = false, bool forceScanWholeMap = false)
            {
                if (Settings.androidsCanConsumeLivingPlants && eater != null &&  eater.IsAndroidTier() && eater == getter)
                {
                    //If not desperate switch on standard way, because otherwise it will cause massive lag
                    if (!desperate)
                    {
                        getter.RaceProps.foodType = Utils.FoodTypeBioGenerator;
                    }
                    else
                        getter.RaceProps.intelligence = Intelligence.Humanlike;
                }

            }
        }

    }
}