using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class CompAndroidSpawnerGeneric : ThingComp
    {
        public SpawnerCompProperties_GenericSpawner Spawnprops
        {
            get
            {
                return this.props as SpawnerCompProperties_GenericSpawner;
            }
        }

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
            PawnGenerationRequest request = new PawnGenerationRequest(this.Spawnprops.Pawnkind, Faction.OfPlayer, PawnGenerationContext.NonPlayer);
            Pawn pawn = PawnGenerator.GeneratePawn(request);


            GenSpawn.Spawn(pawn, parent.Position, parent.Map);
        }

    }
}