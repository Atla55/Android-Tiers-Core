using System;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    // Token: 0x02000022 RID: 34
    public class CompProperties_SpawnPawn : CompProperties_UseEffect
    {
        // Token: 0x0600007A RID: 122 RVA: 0x000053D2 File Offset: 0x000035D2
        public CompProperties_SpawnPawn()
        {
            this.compClass = typeof(CompUseEffect_SpawnAndroid);
        }

        // Token: 0x04000031 RID: 49
        public PawnKindDef pawnKind;

        // Token: 0x04000032 RID: 50
        public int amount = 1;

        // Token: 0x04000033 RID: 51
        public FactionDef forcedFaction;

        // Token: 0x04000034 RID: 52
        public bool usePlayerFaction = true;

        // Token: 0x04000035 RID: 53
        public string pawnSpawnedStringKey = "AndroidSpawnedDroidMessageText";

        // Token: 0x04000036 RID: 54
        public bool sendMessage = true;

    }
}