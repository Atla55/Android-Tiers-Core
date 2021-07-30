using System;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class CompSurrogateSpawner : ThingComp
    {
        public CompProperties_SurrogateSpawner Spawnprops
        {
            get
            {
                return this.props as CompProperties_SurrogateSpawner;
            }
        }

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

    