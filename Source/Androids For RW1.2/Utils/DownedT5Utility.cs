using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public static class DownedT5Utility
    {
        public static Pawn GenerateT5(int tile)
        {
            PawnKindDef AndroidT5Colonist = PawnKindDefOf.AndroidT5Colonist;
            Faction ofplayer = Faction.OfAncients;
            PawnGenerationRequest request = new PawnGenerationRequest(AndroidT5Colonist, ofplayer, PawnGenerationContext.NonPlayer, tile, false, false, false, false, true, false, 20f, true, true, true, false, false, false, false, false, 0f, null, 0f, null, null, null, null);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            HealthUtility.DamageUntilDowned(pawn);
            Hediff hediff = HediffMaker.MakeHediff(MOARANDROIDS.HediffDefOf.RebootingSequenceAT, pawn, null);
            hediff.Severity = 1f;
            pawn.health.AddHediff(hediff, null, null);
            return pawn;
        }
        private const float RelationWithColonistWeight = 0.8f;

        private const float ChanceToRedressWorldPawn = 0f;
    }
}