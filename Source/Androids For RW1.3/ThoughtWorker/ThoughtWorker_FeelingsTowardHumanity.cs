using System;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class ThoughtWorker_FeelingsTowardHumanity : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            TraitDef feel = DefDatabase<TraitDef>.GetNamed("FeelingsTowardHumanity", false);
            if(feel == null)
                return false;

            int num = p.story.traits.DegreeOfTrait(feel);
            bool flag = !p.RaceProps.Humanlike;
            ThoughtState result;
            if (flag)
            {
                result = false;
            }
            else
            {
                bool flag2 = !RelationsUtility.PawnsKnowEachOther(p, other);
                if (flag2)
                {
                    result = false;
                }
                else
                {
                    bool flag3 = other.IsAndroid();
                    //SI androide OU un cyborg
                    if (flag3 || other.health.hediffSet.CountAddedAndImplantedParts() >= 5)
                    {
                        result = false;
                    }
                    else
                    {
                        bool flag4 = num == 1;
                        if (flag4)
                        {
                            result = ThoughtState.ActiveAtStage(0);
                        }
                        else
                        {
                            bool flag5 = num == 2;
                            if (flag5)
                            {
                                result = ThoughtState.ActiveAtStage(1);
                            }
                            else
                            {
                                result = false;
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
