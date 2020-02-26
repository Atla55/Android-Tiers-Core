using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace MOARANDROIDS
{
    public class ThinkNode_ConditionalMustKeepLyingDownM7Surrogate : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        { 
            if (pawn.CurJob == null || !pawn.GetPosture().Laying() || (pawn.def.defName == "M7Mech" &&  !pawn.IsSurrogateAndroid() ))
            {
                return false;
            }
            if (!pawn.Downed)
            {
                if (RestUtility.DisturbancePreventsLyingDown(pawn))
                {
                    return false;
                }
                if (!pawn.CurJob.restUntilHealed || !HealthAIUtility.ShouldSeekMedicalRest(pawn))
                {
                    if (!pawn.jobs.curDriver.asleep)
                    {
                        return false;
                    }
                    if (!pawn.CurJob.playerForced && RestUtility.TimetablePreventsLayDown(pawn))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
