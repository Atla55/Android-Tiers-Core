using System;
using Verse;

namespace MOARANDROIDS
{
    public class SpawnerCompProperties_GenericSpawner : CompProperties
    {
        public SpawnerCompProperties_GenericSpawner()
        {
            this.compClass = typeof(CompAndroidSpawnerGeneric);
        }

        public PawnKindDef Pawnkind;
        public int gender;
    }
}