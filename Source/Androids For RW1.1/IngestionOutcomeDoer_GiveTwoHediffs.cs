using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    class IngestionOutcomeDoer_GiveTwoHediffs : IngestionOutcomeDoer
    {
        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            if (pawn.IsAndroid() == false)
            {
                Hediff hediff = HediffMaker.MakeHediff(this.hediffDef_Human, pawn, null);
                float num;
                if (this.severity > 0f)
                {
                    num = this.severity;
                }
                else
                {
                    num = this.hediffDef_Human.initialSeverity;
                }
                if (this.divideByBodySize)
                {
                    num /= pawn.BodySize;
                }
                hediff.Severity = num;
                pawn.health.AddHediff(hediff, null, null, null);
            }
            else
            {
                Hediff hediff = HediffMaker.MakeHediff(this.hediffDef_Android, pawn, null);
                float num;
                if (this.severity > 0f)
                {
                    num = this.severity;
                }
                else
                {
                    num = this.hediffDef_Android.initialSeverity;
                }
                if (this.divideByBodySize)
                {
                    num /= pawn.BodySize;
                }
                hediff.Severity = num;
                pawn.health.AddHediff(hediff, null, null, null);
            }
        }

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats(ThingDef parentDef)
        {
            if (parentDef.IsDrug && this.chance >= 1f)
            {
                foreach (StatDrawEntry s in this.hediffDef_Human.SpecialDisplayStats(StatRequest.ForEmpty()))
                {
                    yield return s;
                }
            }
            yield break;
        }

        public HediffDef hediffDef_Android;

        public HediffDef hediffDef_Human;

        public float severity = -1f;

        private bool divideByBodySize;
    }
}


