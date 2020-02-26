using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace MOARANDROIDS
{
    public class ThinkNode_ConditionalM7Charging : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        { 
            //QUe les SM7 controlés peuvent se recharger
            if ((!pawn.Downed && pawn.def.defName == Utils.M7 && pawn.IsSurrogateAndroid(true) ))
            {
                //Min 30% pour aller se recharger en auto
                if (pawn.needs.food != null && pawn.needs.food.CurLevelPercentage < 0.3f)
                    return true;
            }
            return false;
        }
    }
}
