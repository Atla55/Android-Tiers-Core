using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class Recipe_InstallArtificialBodyPartAndroid : Recipe_SurgeryAndroids
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
                        IEnumerable<Hediff> diffs = from x in pawn.health.hediffSet.hediffs
                                                    where x.Part == record
                                                    select x;
                        if (diffs.Count<Hediff>() != 1 || diffs.First<Hediff>().def != recipe.addsHediff)
                        {
                            if (record.parent == null || pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined).Contains(record.parent))
                            {
                                if (!pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record) || pawn.health.hediffSet.HasDirectlyAddedPartFor(record))
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
            if (billDoer != null)
            {
                if (base.CheckSurgeryFailAndroid(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }
                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
                {
                    billDoer,
                    pawn
                });
                MedicalRecipesUtility.RestorePartAndSpawnAllPreviousParts(pawn, part, billDoer.Position, billDoer.Map);
            }
            else if (pawn.Map != null)
            {
                MedicalRecipesUtility.RestorePartAndSpawnAllPreviousParts(pawn, part, pawn.Position, pawn.Map);
            }
            else
            {
                pawn.health.RestorePart(part, null, true);
            }
            pawn.health.AddHediff(this.recipe.addsHediff, part, null);
        }
    }
}

