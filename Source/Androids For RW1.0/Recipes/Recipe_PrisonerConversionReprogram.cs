using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    // Token: 0x02000009 RID: 9
    public class Recipe_AndroidRewireSurgery : Recipe_SurgeryAndroids
    {
        // Token: 0x0600000C RID: 12 RVA: 0x000021D8 File Offset: 0x000003D8
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            bool flag = billDoer != null;
            bool flag2 = flag;
            if (flag2)
            {
                bool flag3 = !base.CheckSurgeryFailAndroid(billDoer, pawn, ingredients, part, null);
                bool flag4 = flag3;
                if (flag4)
                {
                    pawn.health.AddHediff(this.recipe.addsHediff, part, null);
                    TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
                    {
                        billDoer,
                        pawn
                    });
                    this.FactionChange(pawn);
                    upper = 20;
                }
                else
                {
                    upper = 5;                 
                }

                this.RandomCorruption(pawn);
            }
        }

        // Token: 0x0600000E RID: 14 RVA: 0x0000229C File Offset: 0x0000049C
        private void FactionChange(Pawn pawn)
        {
            bool flag = pawn.Faction != Faction.OfPlayer;
            bool flag2 = flag;
            if (flag2)
            {
                pawn.SetFaction(Faction.OfPlayer, null);
                string text = "Atlas_FactionChange".Translate(pawn.Name.ToStringShort);
                text = text.AdjustedFor(pawn);
                string label = "LetterLabelAtlas_FactionChange".Translate();
                Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.PositiveEvent, pawn, null);
            }
        }

        private void RandomCorruption(Pawn pawn)
        {
            Random rnd = new Random();
            int chance = rnd.Next(0, upper);

            {
                bool check = chance == 1;
                if (check)
                {
                    pawn.health.AddHediff(HediffDefOf.CorruptMemory, pawn.health.hediffSet.GetBrain(), null);
                }
            }
        }

        int upper;

    }
}
