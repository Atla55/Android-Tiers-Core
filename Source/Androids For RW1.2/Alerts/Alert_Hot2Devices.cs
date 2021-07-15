using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    // Token: 0x020007C0 RID: 1984
    public class Alert_Hot2Devices : Alert
    {
        public Alert_Hot2Devices()
        {
            this.defaultLabel = "ATPP_AlertHot2Devices".Translate();
            this.defaultExplanation = "ATPP_AlertHot2DevicesDesc".Translate();
            this.defaultPriority = AlertPriority.High;
        }

        public override AlertReport GetReport()
        {
            List<Thing> build = null;

            List<Map> maps = Find.Maps;
            for (int i = 0; i < maps.Count; i++)
            {
                if (build == null)
                    build = Utils.GCATPP.getHeatSensitiveDevicesByHotLevel(maps[i], 2);
                else
                    build.AddRange(build);
            }

            if (build != null)
                return AlertReport.CulpritsAre(build);
            else
                return false;
        }
    }
}
