using System.Linq;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class PawnRelationWorker_Creation : PawnRelationWorker
    {
            public override bool InRelation(Pawn me, Pawn other)
            {
                return me != other;
            }
    }
}
