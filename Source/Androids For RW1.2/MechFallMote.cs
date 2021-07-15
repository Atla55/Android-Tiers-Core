using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    // Token: 0x0200068D RID: 1677
    public static class MechFallMoteMaker
    {
        // Token: 0x060022A7 RID: 8871 RVA: 0x00103CA4 File Offset: 0x001020A4
        public static void MakeMechFallMote(IntVec3 cell, Map map)
        {
            Mote mote = (Mote)ThingMaker.MakeThing(RimWorld.ThingDefOf.Mote_Bombardment, null);
            mote.exactPosition = cell.ToVector3Shifted();
            mote.Scale = 5f;
            mote.rotationRate = 0f;
            GenSpawn.Spawn(mote, cell, map);
        }

    }
}
