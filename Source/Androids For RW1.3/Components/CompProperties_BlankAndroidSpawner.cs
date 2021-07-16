using System;
using Verse;

namespace MOARANDROIDS
{
    public class CompProperties_BlankAndroidSpawner : CompProperties
    {
        public CompProperties_BlankAndroidSpawner()
        {
            this.compClass = typeof(CompBlankAndroidSpawner);
        }

        public PawnKindDef Pawnkind;
    }
}
