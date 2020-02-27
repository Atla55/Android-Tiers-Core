using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    [DefOf]
    public static class BodyPartTagDefOf
    {
        static BodyPartTagDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BodyPartTagDefOf));
        }

        public static BodyPartTagDef CPSource;

        public static BodyPartTagDef HVSource;

        public static BodyPartTagDef EVKidney;

        public static BodyPartTagDef EVLiver;

    }
}
