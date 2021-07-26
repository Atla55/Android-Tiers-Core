using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;


/*
 * Used only to avoid patching some vanilla health methods to prevent issues (needs.rest == NULL)
 */
namespace MOARANDROIDS
{
    public class Need_Rest_Fake : Need_Rest
    {
        public Need_Rest_Fake(Pawn pawn) : base(pawn)
        {
        }

        public override void NeedInterval()
        {

        }
    }
}
