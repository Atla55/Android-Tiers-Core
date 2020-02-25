using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class Recipe_SurgeryAndroids : RecipeWorker
    {
        protected bool CheckSurgeryFailAndroid(Pawn surgeon, Pawn patient, List<Thing> ingredients, BodyPartRecord part, Bill bill)
        {
            float num = 1f;
            num *= StatExtension.GetStatValue(surgeon, StatDefOf.AndroidSurgerySuccessChance, true);
            if (RestUtility.InBed(patient))
            {
                num *= patient.CurrentBed().GetStatValue(StatDefOf.AndroidSurgerySuccessChance, true);
            }
            num *= this.recipe.surgerySuccessChanceFactor;
            if (!Rand.Chance(num))
            {
                if (Rand.Chance(this.recipe.deathOnFailedSurgeryChance))
                {
                    HealthUtility.GiveInjuriesOperationFailureCatastrophic(patient, part);
                    if (!patient.Dead)
                    {
                        patient.Kill(null, null);
                    }
                    Messages.Message("MessageMedicalOperationFailureFatalAndroid".Translate(surgeon.LabelShort, patient.LabelShort, this.recipe.label), patient, MessageTypeDefOf.NegativeHealthEvent);
                }
                else if (Rand.Chance(0.5f))
                {
                    if (Rand.Chance(0.1f))
                    {
                        Messages.Message("MessageMedicalOperationFailureRidiculousAndroid".Translate(surgeon.LabelShort, patient.LabelShort), patient, MessageTypeDefOf.NegativeHealthEvent);
                        HealthUtility.GiveInjuriesOperationFailureRidiculous(patient);
                    }
                    else
                    {
                        Messages.Message("MessageMedicalOperationFailureCatastrophicAndroid".Translate(surgeon.LabelShort, patient.LabelShort), patient, MessageTypeDefOf.NegativeHealthEvent);
                        HealthUtility.GiveInjuriesOperationFailureCatastrophic(patient, part);
                    }
                }
                else
                {
                    Messages.Message("MessageMedicalOperationFailureMinorAndroid".Translate(surgeon.LabelShort, patient.LabelShort), patient, MessageTypeDefOf.NegativeHealthEvent);
                    HealthUtility.GiveInjuriesOperationFailureMinor(patient, part);
                }
                if (!patient.Dead)
                {
                    this.TryGainBotchedSurgeryThought(patient, surgeon);
                }
                return true;
            }
            return false;
        }

        // Token: 0x06001266 RID: 4710 RVA: 0x0008C4AD File Offset: 0x0008A8AD
        private void TryGainBotchedSurgeryThought(Pawn patient, Pawn surgeon)
        {
            if (!patient.RaceProps.Humanlike)
            {
                return;
            }
            patient.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.BotchedMyUpgrade, surgeon);
        }


        // Token: 0x04000B58 RID: 2904
        private const float CatastrophicFailChance = 0.5f;

        // Token: 0x04000B59 RID: 2905
        private const float RidiculousFailChanceFromCatastrophic = 0.1f;

        // Token: 0x04000B5A RID: 2906
        private const float InspiredSurgeryFailChanceFactor = 0.1f;
    }
}
