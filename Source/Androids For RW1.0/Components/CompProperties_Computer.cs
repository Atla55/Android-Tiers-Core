using System;
using Verse;
using RimWorld;


namespace MOARANDROIDS
{
    public class CompProperties_Computer : CompProperties
    {
        public CompProperties_Computer()
        {
            this.compClass = typeof(CompComputer);
        }

        public string ambiance;
        public bool isSecurityServer = false;
        public string type = "Computer";
    }
}
