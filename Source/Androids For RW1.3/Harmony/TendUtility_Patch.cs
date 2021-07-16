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
    internal class TendUtility_Patch

    {
        /*
         * Set correct stats for crafter healing androids
         */
        [HarmonyPatch(typeof(TendUtility), "CalculateBaseTendQuality")]
        [HarmonyPatch(new Type[] { typeof(Pawn), typeof(Pawn), typeof(float), typeof(float) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal })]
        public class CalculateBaseTendQuality_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn doctor, Pawn patient, float medicinePotency, float medicineQualityMax, ref float __result)
            {
                if (!Settings.androidsCanOnlyBeHealedByCrafter || !patient.IsAndroidTier())
                    return true;

                try
                {
                    float num;
                    if (doctor != null)
                    {
                        num = doctor.GetStatValue(Utils.statDefAndroidTending, true);
                    }
                    else
                    {
                        num = 0.75f;
                    }
                    num *= medicinePotency;
                    Building_Bed building_Bed = (patient == null) ? null : patient.CurrentBed();
                    if (building_Bed != null)
                    {
                        num += building_Bed.GetStatValue(StatDefOf.MedicalTendQualityOffset, true);
                    }
                    if (doctor == patient && doctor != null)
                    {
                        num *= 0.7f;
                    }
                    __result = Mathf.Clamp(num, 0f, medicineQualityMax);
                    return false;
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] TendUtility.CalculateBaseTendQuality " + e.Message + " " + e.StackTrace);
                    return true;
                }
            }
        }
    }
}