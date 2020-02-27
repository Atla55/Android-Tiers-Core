using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class CompAndroidSpawner1T : ThingComp
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
                PawnKindDefOf.AndroidT1Colonist
            }.RandomElement<PawnKindDef>();
            PawnGenerationRequest request = new PawnGenerationRequest(pawnKindDef, Faction.OfPlayer, PawnGenerationContext.NonPlayer);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            
            //TODO: Implement, make wor k, test.
            //Pawn originalCloned = parent.TryGetComp<ThingyHolderThatsHoldingAClonedPawn>();
            //pawn.story = originalCloned.story;

            GenSpawn.Spawn(pawn, parent.Position, parent.Map);
        }
    }
}