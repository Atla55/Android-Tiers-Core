using System;
using RimWorld.Planet;
using RimWorld;

namespace MOARANDROIDS
{
    public class WorldObjectCompProperties_DownedT5Android : WorldObjectCompProperties
    {
        public WorldObjectCompProperties_DownedT5Android()
        {
            this.compClass = typeof(DownedT5AndroidComp);
            this.compClass = typeof(TimedForcedExit);
        }
    }
}