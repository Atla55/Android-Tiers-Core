using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class CompAndroidSpawnerLightSwarm : ThingComp
    {

        public override void CompTick()
        {
            this.CheckShouldSpawn();
        }

        private void CheckShouldSpawn()
        {
            if (true)
            {
                this.SpawnDude();
                this.parent.Destroy();
            }
        }

        public void SpawnDude()
        {
            PawnKindDef pawnKindDef = new List<PawnKindDef>
            {
                PawnKindDefOf.MicroScyther
            }.RandomElement<PawnKindDef>();
            PawnGenerationRequest request = new PawnGenerationRequest(pawnKindDef, Faction.OfPlayer, PawnGenerationContext.NonPlayer);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            GenSpawn.Spawn(pawn, parent.Position, parent.Map);
            pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterNotColony, null, true, false, null);

            Hediff hediff = HediffMaker.MakeHediff(MOARANDROIDS.HediffDefOf.BatteryChargeMech, pawn, null);
            hediff.Severity = 0.5f;
            pawn.health.AddHediff(hediff, null, null);
        }
    }
}