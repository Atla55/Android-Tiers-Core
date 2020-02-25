using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using System.Linq;

namespace MOARANDROIDS
{
    public class Recipe_RerollTraits : Recipe_SurgeryAndroids
    {
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            for (int i = 0; i < recipe.appliedOnFixedBodyParts.Count; i++)
            {
                BodyPartDef part = recipe.appliedOnFixedBodyParts[i];
                List<BodyPartRecord> bpList = pawn.RaceProps.body.AllParts;
                for (int j = 0; j < bpList.Count; j++)
                {
                    BodyPartRecord record = bpList[j];
                    if (record.def == part)
                    {
                        if (pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined).Contains(record))
                        {
                            if (!pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record))
                            {
                                if (!pawn.health.hediffSet.hediffs.Any((Hediff x) => x.Part == record && x.def == recipe.addsHediff))
                                {
                                    yield return record;
                                }
                            }
                        }
                    }
                }
            }
            yield break;
        }
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
                    this.RerollTraits(pawn, pawn.story.traits.allTraits);
                    upper = 40;
                }
                else
                {
                    upper = 10;
                }

                this.RandomCorruption(pawn);
            }
        }

        private void RerollTraits(Pawn pawn, List<Trait> traits)
        {
            for (int i = traits.Count - 1; i >= 0; i--)
            {
                traits.Remove(traits[i]);
            }

            for (int i = 0; i < 3; i++)
            {
                DefDatabase<TraitDef>.AllDefs.TryRandomElement<TraitDef>(out TraitDef traitDef);

                int num = PawnGenerator.RandomTraitDegree(traitDef);
                Trait trait = new Trait(traitDef, num, false);
                if (traits.Contains(trait))
                {
                    i--;
                }
                else
                {
                    traits.Add(trait);
                }         

            }
            string text = "Atlas_TraitReroll".Translate(pawn.Name.ToStringShort).AdjustedFor(pawn);

            string label = "LetterLabelAtlas_TraitReroll".Translate();
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NeutralEvent, pawn, null);
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
