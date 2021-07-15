using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    [DefOf]
    public static class FactionDefOf
    {
        static FactionDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(FactionDefOf));
        }

        public static FactionDef AndroidFriendliesAtlas;

        public static FactionDef PlayerColonyAndroid;
    }

}

