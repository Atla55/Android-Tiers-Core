using System;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    // Token: 0x02000020 RID: 32
    public class CompSurrogateSpawner : ThingComp
    {
        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000041 RID: 65 RVA: 0x00003478 File Offset: 0x00001678
        public CompProperties_SurrogateSpawner Spawnprops
        {
            get
            {
                return this.props as CompProperties_SurrogateSpawner;
            }
        }

        // Token: 0x06000042 RID: 66 RVA: 0x00003495 File Offset: 0x00001695
        public override void CompTick()
        {
            this.Spawn();
            this.parent.Destroy(DestroyMode.Vanish);
        }
        
        public void Spawn()
        {
            try
            {
                Utils.forceGeneratedAndroidToBeDefaultPainted = true;
                Utils.generateSurrogate(Faction.OfPlayer,this.Spawnprops.Pawnkind, this.parent.Position,this.parent.Map, true, false, -1,false,false,this.Spawnprops.gender);
                Utils.forceGeneratedAndroidToBeDefaultPainted = false;
                //Utils.GCATPP.pushSurrogateAndroid(surrogate);
            }
            catch(Exception e)
            {
                Log.Message("Surrogate Android Spawn Error : " + e.Message + " " + e.StackTrace);
            }
        }
    }
}

    