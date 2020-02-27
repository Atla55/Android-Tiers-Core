using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class CompAndroidSpawnerDoggo : ThingComp
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
                PawnKindDefOf.AndroidDog
            }.RandomElement<PawnKindDef>();
            PawnGenerationRequest request = new PawnGenerationRequest(pawnKindDef, Faction.OfPlayer, PawnGenerationContext.NonPlayer);
            Pawn pawn = PawnGenerator.GeneratePawn(request);

            //TODO: Implement, make work, test.
            //Pawn originalCloned = parent.TryGetComp<ThingyHolderThatsHoldingAClonedPawn>();
            //pawn.story = originalCloned.story;

            GenSpawn.Spawn(pawn, parent.Position, parent.Map);
        }
    }
}