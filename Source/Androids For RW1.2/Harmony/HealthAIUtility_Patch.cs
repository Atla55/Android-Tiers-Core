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
    internal class HealthAIUtility_Patch
    {
        /*
         * Permet de forcer les surrogates downed d'etres entreposés dans leurs pods dédiés
         */
        [HarmonyPatch(typeof(HealthAIUtility), "ShouldSeekMedicalRest")]
        public class ShouldSeekMedicalRest_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn pawn, ref bool __result)
            {
                try
                {
                    if (pawn.Faction == Faction.OfPlayer)
                    {
                        CompAndroidState cas = pawn.TryGetComp<CompAndroidState>();
                        if (cas != null && pawn.health != null && pawn.health.summaryHealth.SummaryHealthPercent >= 0.80f && cas.isSurrogate && cas.surrogateController == null && pawn.ownership != null && pawn.ownership.OwnedBed != null )//&& ReachabilityUtility.CanReach(pawn, pawn.ownership.OwnedBed, PathEndMode.OnCell, Danger.Deadly))
                        {
                            __result = false;
                        }
                    }
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] HealthAIUtility.ShouldSeekMedicalRest " + e.Message + " " + e.StackTrace);
                }
            }
        }


        [HarmonyPatch(typeof(HealthAIUtility), "FindBestMedicine")]
        public class FindBestMedicine_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn healer, Pawn patient, ref Thing __result)
            {
                try
                {
                    //On ne soccupe que des patient étant des androids
                    /*if (!)
                        return true;*/


                    if (Settings.androidsCanUseOrganicMedicine)
                        return;

                    bool patientIsAndroid = Utils.ExceptionAndroidList.Contains(patient.def.defName) || patient.IsCyberAnimal();

                    if (patient.playerSettings == null || patient.playerSettings.medCare <= MedicalCareCategory.NoMeds)
                    {
                        __result = null;
                        return;
                    }
                    if (Medicine.GetMedicineCountToFullyHeal(patient) <= 0)
                    {
                        __result = null;
                        return;
                    }
                    Predicate<Thing> predicate;

                    //COmpatibilité avec pharmacist, le medoc renvoyé doit avoir une quantitée de soin inferieur ou egal à celui renvoyé par les appels précédents
                    float medicalPotency = 0;
                    if(__result != null)
                    {
                        medicalPotency = __result.def.GetStatValueAbstract(StatDefOf.MedicalPotency, null);
                    }

                    if(patientIsAndroid)
                        predicate = (Thing m) => Utils.ExceptionNanoKits.Contains(m.def.defName) && m.def.GetStatValueAbstract(StatDefOf.MedicalPotency, null)  <= medicalPotency && !m.IsForbidden(healer) && patient.playerSettings.medCare.AllowsMedicine(m.def) && healer.CanReserve(m, 10, 1, null, false);
                    else
                        predicate = (Thing m) => !Utils.ExceptionNanoKits.Contains(m.def.defName) && m.def.GetStatValueAbstract(StatDefOf.MedicalPotency, null) <= medicalPotency && !m.IsForbidden(healer) && !m.IsForbidden(healer) && patient.playerSettings.medCare.AllowsMedicine(m.def) && healer.CanReserve(m, 10, 1, null, false);

                    Func<Thing, float> priorityGetter = (Thing t) => t.def.GetStatValueAbstract(StatDefOf.MedicalPotency, null);

                    IntVec3 position = patient.Position;
                    Map map = patient.Map;
                    List<Thing> searchSet = patient.Map.listerThings.ThingsInGroup(ThingRequestGroup.Medicine);
                    PathEndMode peMode = PathEndMode.ClosestTouch;
                    TraverseParms traverseParams = TraverseParms.For(healer, Danger.Deadly, TraverseMode.ByPawn, false);
                    Predicate<Thing> validator = predicate;
                    __result = GenClosest.ClosestThing_Global_Reachable(position, map, searchSet, peMode, traverseParams, 9999f, validator, priorityGetter);
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] HealthAIUtility.FindBestMedicine(Error) : " + e.Message + " - " + e.StackTrace);
                }
            }
        }
    }
}