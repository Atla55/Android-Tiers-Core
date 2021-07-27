using System;
using Verse;
using RimWorld;

//Extension method, in RimWorld namespace
namespace RimWorld
{ 
    //Make sure I bind to the Android fleshDef as early as possible AFTER they are loaded
    [StaticConstructorOnStartup]
    static public class PawnExt
    {
        static readonly public FleshTypeDef androidFlesh;


        static PawnExt()
        {
            androidFlesh = DefDatabase<FleshTypeDef>.GetNamed("AndroidTier");
        }
    
        static public bool IsAndroid(this Pawn pawn)
        {
            return pawn.RaceProps.FleshType == androidFlesh;
        }

        static public bool IsNotAndroid(this Pawn pawn)
        {
            return pawn.RaceProps.FleshType != androidFlesh;
        }
    }
}
