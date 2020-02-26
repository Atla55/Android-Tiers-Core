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
                Utils.forceGeneratedAndroidToBeDefaultPainted = true;
                Utils.PawnInventoryGeneratorCanHackInvNutritionValue = false;
                this.SpawnDude();
                Utils.forceGeneratedAndroidToBeDefaultPainted = false;
                Utils.PawnInventoryGeneratorCanHackInvNutritionValue = true;

                this.parent.Destroy();
            }
        }

        public void SpawnDude()
        {
            Gender gender = default(Verse.Gender);
            if (this.Spawnprops.gender != -1)
            {
                if (this.Spawnprops.gender == 0)
                    gender = Verse.Gender.Male;
                else
                    gender = Verse.Gender.Female;
            }

            PawnGenerationRequest request = new PawnGenerationRequest(this.Spawnprops.Pawnkind, Faction.OfPlayer, PawnGenerationContext.NonPlayer, fixedGender : gender);
            Pawn pawn = PawnGenerator.GeneratePawn(request);


            GenSpawn.Spawn(pawn, parent.Position, parent.Map);
        }

    }
}