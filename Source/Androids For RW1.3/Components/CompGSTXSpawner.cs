using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class CompGSTXSpawner : ThingComp
    {
        public CompProperties_GSTXSpawner Spawnprops
        {
            get
            {
                return this.props as CompProperties_GSTXSpawner;
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
                //Spawn GenomeSequencer programmer pour le modele d'androide specifié
                ThingDef td = null;
                td = DefDatabase<ThingDef>.GetNamed(Spawnprops.GSThing,false);
                string pawnKind = "";
                Gender gender = Gender.Male;
                string source = "";

                if (td != null)
                {
                    Thing thing = ThingMaker.MakeThing(td);
                    GenSpawn.Spawn(thing, this.parent.Position, this.parent.Map);

                    if(Spawnprops.GSThing == "ATPP_GS_TX2KMale")
                    {
                        pawnKind = "ATPP_Android2KTXKind";
                        gender = Gender.Male;
                        source = "TX2K";
                    }
                    else if (Spawnprops.GSThing == "ATPP_GS_TX2KFemale")
                    {
                        pawnKind = "ATPP_Android2KTXKind";
                        gender = Gender.Female;
                        source = "TX2K Surrogate";
                    }
                    else if (Spawnprops.GSThing == "ATPP_GS_TX3Male")
                    {
                        pawnKind = "ATPP_Android3TXKind";
                        gender = Gender.Male;
                        source = "TX3";
                    }
                    else if (Spawnprops.GSThing == "ATPP_GS_TX3Female")
                    {
                        pawnKind = "ATPP_Android3TXKind";
                        gender = Gender.Female;
                        source = "TX3";
                    }
                    else if (Spawnprops.GSThing == "ATPP_GS_TX4Male")
                    {
                        pawnKind = "ATPP_Android4TXKind";
                        gender = Gender.Male;
                        source = "TX4";
                    }
                    else if (Spawnprops.GSThing == "ATPP_GS_TX4Female")
                    {
                        pawnKind = "ATPP_Android4TXKind";
                        gender = Gender.Female;
                        source = "TX4";
                    }

                    if(Spawnprops.surrogate == 1)
                    {
                        source += " (Surrogate)";
                    }

                    Traverse baseThing = Traverse.Create(thing);

                    baseThing.Field("isAlien").SetValue(true);
                    baseThing.Field("crownTypeAlien").SetValue("Average_Normal");
                    baseThing.Field("bodyType").SetValue(BodyTypeDefOf.Male);
                    baseThing.Field("pawnKindDef").SetValue(DefDatabase<PawnKindDef>.GetNamed(pawnKind,false));
                    baseThing.Field("gender").SetValue(gender);
                    baseThing.Field("sourceName").SetValue(source);
                    
                    //Utils.GCATPP.pushSurrogateAndroid(surrogate);
                }
            }
            catch(Exception e)
            {
                Log.Message("QEE GenomeSequenser Spawn Error : " + e.Message + " " + e.StackTrace);
            }
        }
    }
}

    