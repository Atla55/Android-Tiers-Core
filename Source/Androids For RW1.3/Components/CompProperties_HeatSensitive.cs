using System;
using Verse;
using RimWorld;


namespace MOARANDROIDS
{
    public class CompProperties_HeatSensitive : CompProperties
    {
        public CompProperties_HeatSensitive()
        {
            this.compClass = typeof(CompHeatSensitive);
        }

        public float hot1 = 20;
        public float hot2 = 30;
        public float hot3 = 35;

        public SoundDef hotSoundDef;
    }
}
