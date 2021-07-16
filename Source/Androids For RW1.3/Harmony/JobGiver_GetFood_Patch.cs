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
    internal class JobGiver_GetFood_Patch
    {

        /*
         * Postfix permettant de rerouter le JobGiver_GetFood en JobDriver_GoReloadBattery
         */
        [HarmonyPatch(typeof(JobGiver_GetFood), "TryGiveJob")]
        public class TryGiveJob_Patch
        {
            /*[HarmonyPrefix]
            public static bool Listene2r(Pawn pawn, ref Job __result, HungerCategory ___minCategory, bool ___forceScanWholeMap)
            {
                Need_Food food = pawn.needs.food;
                if (food == null || food.CurCategory < ___minCategory)
                {
                    __result =null;
                    return false;
                }
                bool flag;
                if (pawn.AnimalOrWildMan())
                {
                    flag = true;
                }
                else
                {
                    Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition, false);
                    flag = (firstHediffOfDef != null && firstHediffOfDef.Severity > 0.4f);
                }
                bool flag2 = pawn.needs.food.CurCategory == HungerCategory.Starving;
                bool desperate = flag2;
                bool canRefillDispenser = true;
                bool canUseInventory = true;
                bool allowCorpse = flag;
                bool flag3 = ___forceScanWholeMap;
                Thing thing;
                ThingDef thingDef;
                if (!FoodUtility.TryFindBestFoodSourceFor(pawn, pawn, desperate, out thing, out thingDef, canRefillDispenser, canUseInventory, false, allowCorpse, false, pawn.IsWildMan(), flag3))
                {
                    __result= null;
                    return false;
                }

                Pawn pawn2 = thing as Pawn;
                if (pawn2 != null)
                {
                    __result  = new Job(JobDefOf.PredatorHunt, pawn2)
                    {
                        killIncappedTarget = true
                    };
                    return false;
                }
                if (thing is Plant && thing.def.plant.harvestedThingDef == thingDef)
                {
                    __result = new Job(JobDefOf.Harvest, thing);
                    return false;
                }
                Building_NutrientPasteDispenser building_NutrientPasteDispenser = thing as Building_NutrientPasteDispenser;
                if (building_NutrientPasteDispenser != null && !building_NutrientPasteDispenser.HasEnoughFeedstockInHoppers())
                {
                    Building building = building_NutrientPasteDispenser.AdjacentReachableHopper(pawn);
                    if (building != null)
                    {
                        ISlotGroupParent hopperSgp = building as ISlotGroupParent;
                        Job job = WorkGiver_CookFillHopper.HopperFillFoodJob(pawn, hopperSgp);
                        if (job != null)
                        {
                            __result = job;
                            return false;
                        }
                    }
                    thing = FoodUtility.BestFoodSourceOnMap(pawn, pawn, flag2, out thingDef, FoodPreferability.MealLavish, false, !pawn.IsTeetotaler(), false, false, false, false, false, false, ___forceScanWholeMap);
                    if (thing == null)
                    {
                        __result = null;
                        return false;
                    }
                }
                float nutrition = FoodUtility.GetNutrition(thing, thingDef);
                __result = new Job(JobDefOf.Ingest, thing)
                {
                    count = FoodUtility.WillIngestStackCountOf(pawn, thingDef, nutrition)
                };
                return false;

            }*/

            [HarmonyPostfix]
            public static void Listener(Pawn pawn, ref Job __result)
            {
                try
                {
                    //Si android alors OK
                    if (Utils.ExceptionAndroidCanReloadWithPowerList.Contains(pawn.def.defName))
                    {
                        //Check si l'android utilise sa batterie le cas non echeant on arrete l'override ET on l'arret aussi si l'android dans une caravane !!
                        CompAndroidState ca = pawn.TryGetComp<CompAndroidState>();
                        if (ca == null || !pawn.Spawned || !ca.UseBattery || pawn.Drafted)
                            return;

                        //SI recharge LWPN en cours valide alors on annule la recharge par nourrite ou elec traditionelle
                        if(Utils.POWERPP_LOADED && ca.connectedLWPNActive && ca.connectedLWPN != null)
                        {
                            __result = null;
                            return;
                        }


                        Building_Bed pod = null;
                        //Check disponibilité d'un POD alimenté
                        try
                        {
                            pod = Utils.getAvailableAndroidPodForCharging(pawn, pawn.def.defName == "M7Mech");
                        }
                        catch (Exception)
                        {
                        }
                        
                        if(pod != null)
                        {
                            __result = new Job(DefDatabase<JobDef>.GetNamed("ATPP_GoReloadBattery"), new LocalTargetInfo(pod));
                            return;
                        }

                        //Log.Message("Android want EAT !!! ");
                        //Recherche reload station disponible sur la map 
                        Building rsb = Utils.GCATPP.getFreeReloadStation(pawn.Map, pawn);
                        if (rsb == null)
                        {
                            __result = null;
                            //Log.Message("No ReloadStation found !!");
                            return;
                        }
                        //Obtention place disponible sur la RS
                        CompReloadStation rs = rsb.TryGetComp<CompReloadStation>();

                        if (rs == null)
                        {
                            __result = null;
                            //Log.Message("No Place available on ReloadStation");
                            return;
                        }

                        __result = new Job(DefDatabase<JobDef>.GetNamed("ATPP_GoReloadBattery"), new LocalTargetInfo(rs.getFreeReloadPlacePos(pawn)), new LocalTargetInfo(rsb));
                    }
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] JobGiver_GetFood.TryGiveJob : " + e.Message + " - " + e.StackTrace);
                }
            }
        }
    }
}