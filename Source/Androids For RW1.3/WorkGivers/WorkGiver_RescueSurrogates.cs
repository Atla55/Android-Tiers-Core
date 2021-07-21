using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MOARANDROIDS
{
    public class WorkGiver_RescueSurrogates : WorkGiver_RescueDowned
    {
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return Utils.listerDownedSurrogatesThing;
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            List<CompAndroidState> list = Utils.listerDownedSurrogatesCAS;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null && list[i].parent.Spawned)
                {
                    Pawn surrogate = (Pawn)list[i].parent;
                    if (surrogate.Downed && !surrogate.InBed() && list[i].isSurrogate && list[i].surrogateController == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
