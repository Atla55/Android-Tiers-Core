using System;
using RimWorld;

namespace MOARANDROIDS
{
        [DefOf]
        public static class TraitDefOf
        {
            static TraitDefOf()
            {
                DefOfHelper.EnsureInitializedInCtor(typeof(TraitDefOf));
            }

            public static TraitDef FeelingsTowardHumanity;

            public static TraitDef Transhumanist;
    }
}
