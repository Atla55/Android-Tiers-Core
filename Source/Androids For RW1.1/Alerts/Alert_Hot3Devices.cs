using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class Alert_Hot3Devices : Alert_Critical
    {
        public Alert_Hot3Devices()
        {
            this.defaultLabel = "ATPP_AlertHot3Devices".Translate();
            this.defaultExplanation = "ATPP_AlertHot3DevicesDesc".Translate();
            this.defaultPriority = AlertPriority.Critical;
        }

        public override AlertReport GetReport()
        {
            List<Thing> build = null;

            List<Map> maps = Find.Maps;
            for (int i = 0; i < maps.Count; i++)
            {
                if (build == null)
                    build = Utils.GCATPP.getHeatSensitiveDevicesByHotLevel(maps[i], 3);
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
