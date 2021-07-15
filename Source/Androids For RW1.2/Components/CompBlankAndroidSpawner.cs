using System;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class CompBlankAndroidSpawner : ThingComp
    {
        public CompProperties_BlankAndroidSpawner Spawnprops
        {
            get
            {
                return this.props as CompProperties_BlankAndroidSpawner;
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

                PawnGenerationRequest request = new PawnGenerationRequest(Spawnprops.Pawnkind, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, true, 1f, false, true, false, false, false, false, false, false, 0f, null, 0f, null, null, null, null);
                Pawn blankAndroid = PawnGenerator.GeneratePawn(request);
                GenSpawn.Spawn(blankAndroid, this.parent.Position, this.parent.Map, WipeMode.Vanish);

                blankAndroid.health.AddHediff(Utils.hediffBlankAndroid);

                CompAndroidState cas = blankAndroid.TryGetComp<CompAndroidState>();
                cas.isBlankAndroid = true;
                

                Utils.initBodyAsSurrogate(blankAndroid, false);

                string sn = "";
                if (blankAndroid.def.defName == Utils.T3)
                    sn = "T3";
                else if (blankAndroid.def.defName == Utils.T4)
                    sn = "T4";

                blankAndroid.Name = new NameTriple("", sn, "");


                Utils.forceGeneratedAndroidToBeDefaultPainted = false;
                //Utils.GCATPP.pushSurrogateAndroid(surrogate);
            }
            catch(Exception e)
            {
                Log.Message("Blank Android Spawn Error : " + e.Message + " " + e.StackTrace);
            }
        }
    }
}

    