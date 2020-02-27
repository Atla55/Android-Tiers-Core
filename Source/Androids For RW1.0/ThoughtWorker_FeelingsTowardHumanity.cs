using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class ThoughtWorker_FeelingsTowardHumanity : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            int num = p.story.traits.DegreeOfTrait(TraitDefOf.FeelingsTowardHumanity);

            if (!p.RaceProps.Humanlike)
            {
                return false;
            }
            if (!RelationsUtility.PawnsKnowEachOther(p, other))
            {
                return false;
            }
            if (other.IsAndroid() == true)
            {
                return false;
            }
                if (num == 1)
                {
                    return ThoughtState.ActiveAtStage(0);
                }
                if (num == 2)
                {
                    return ThoughtState.ActiveAtStage(1);
                }
            return false;
        }
    }
}

