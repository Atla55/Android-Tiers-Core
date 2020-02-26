using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    // Token: 0x02000441 RID: 1089
    public class Recipe_InstallImplantAndroid : Recipe_SurgeryAndroids
    {
        // Token: 0x06001252 RID: 4690 RVA: 0x0008CB84 File Offset: 0x0008AF84
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

        // Token: 0x06001253 RID: 4691 RVA: 0x0008CBB0 File Offset: 0x0008AFB0
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
            }
            pawn.health.AddHediff(this.recipe.addsHediff, part, null);
        }
    }
}
