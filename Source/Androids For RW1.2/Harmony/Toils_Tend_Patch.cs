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
    internal class Toils_Tend_Patch

    {
        /*
         * PostFix évitant d'attribuer de need comfort et outdoor aux T1 et T2 et l'hygiene a l'ensemble des robots
         */
        [HarmonyPatch(typeof(Toils_Tend), "FinalizeTend")]
        public class FinalizeTend_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn patient, ref Toil __result)
            {
                try
                {
                    if (Settings.androidsCanOnlyBeHealedByCrafter && (patient.IsAndroidTier() || patient.IsCyberAnimal()))
                    {
                        Toil toil = new Toil();
                        toil.initAction = delegate
                        {
                            Pawn actor = toil.actor;
                            Medicine medicine = (Medicine)actor.CurJob.targetB.Thing;
                            float num = (!patient.RaceProps.Animal) ? 500f : 175f;
                            float num2 = (medicine != null) ? medicine.def.MedicineTendXpGainFactor : 0.5f;
                            actor.skills.Learn(SkillDefOf.Crafting, num * num2, false);
                            TendUtility.DoTend(actor, patient, medicine);
                            if (medicine != null && medicine.Destroyed)
                            {
                                actor.CurJob.SetTarget(TargetIndex.B, LocalTargetInfo.Invalid);
                            }
                            if (toil.actor.CurJob.endAfterTendedOnce)
                            {
                                actor.jobs.EndCurrentJob(JobCondition.Succeeded, true);
                            }
                        };
                        toil.defaultCompleteMode = ToilCompleteMode.Instant;
                        __result = toil;

                        return false;
                    }
                    return true;
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] Toils_Tend.FinalizeTend " + e.Message + " " + e.StackTrace);
                    return true;
                }
            }
        }
    }
}